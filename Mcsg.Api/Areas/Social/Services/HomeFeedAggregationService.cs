using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Mcsg.Api.Areas.Social.Services;

using Common.Core.Enums;
using Common.Core.Requests;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;
using Mcsg.Api.Areas.TapShow.Interfaces;
using ComicPost = Mcsg.Api.Areas.Comic.Interfaces.IPostService;
using DocumentPost = Mcsg.Api.Areas.Document.Interfaces.IPostService;
using StoryPost = Mcsg.Api.Areas.Story.Interfaces.IPostService;

/// <summary>
/// Replaces the client-side fan-out (latest-posts-by-type + 3-4 get-*-by-list-id calls) with one server-side pass.
/// Hydrates Feed / Story / Comic / Document / TapShow; any other PostType is logged and returned with Data = null.
/// Rankings (and whole anonymous pages) are cached in Redis through HomeFeedCache.
/// Area services are called sequentially on purpose: they share one scoped IDbConnection/DbContext, which is not thread-safe.
/// </summary>
public class HomeFeedAggregationService : IHomeFeedAggregationService
{
    #region -- Methods --

    public HomeFeedAggregationService(
        IPostService socialPostService,
        IFeedService socialFeedService,
        StoryPost storyPostService,
        ComicPost comicPostService,
        DocumentPost documentPostService,
        ITapShowPostService tapShowPostService,
        HomeFeedCache cache,
        IOptions<Microsoft.AspNetCore.Mvc.JsonOptions> jsonOptions,
        ILogger<HomeFeedAggregationService> logger)
    {
        _socialPostService = socialPostService;
        _socialFeedService = socialFeedService;
        _storyPostService = storyPostService;
        _comicPostService = comicPostService;
        _documentPostService = documentPostService;
        _tapShowPostService = tapShowPostService;
        _cache = cache;
        _jsonOptions = jsonOptions.Value.JsonSerializerOptions;
        _logger = logger;
    }

    public async Task<LatestPostsDetailResponse> GetLatestPostsWithDetail(HttpContext hc)
    {
        // One request object for the whole pass: the ctor logs every header, so avoid building one per area
        var req = new PaginatedR(hc);

        var ranked = await _socialPostService.GetLatestPostsByType(req);
        return await HydrateAsync(ranked, req);
    }

    public async Task<LatestPostsDetailResponse> GetHomeFeedWithDetail(PostHomeFeedR request)
    {
        var pageNumber = request.PageNumber < 1 ? 1 : request.PageNumber;
        var pageSize = request.PageSize < 1 ? PostService.DefaultHomeFeedPageSize : Math.Min(request.PageSize, PostService.MaxHomeFeedPageSize);

        var ranked = await GetRankedPageAsync(request, pageNumber, pageSize);
        var response = await HydrateAsync(ranked, request);
        response.PageNumber = pageNumber;
        response.PageSize = pageSize;
        return response;
    }

    public async Task<string> GetHomeFeedJson(PostHomeFeedR request)
    {
        // Anonymous pages are identical for every guest of the same platform: cache the whole hydrated page.
        // Logged-in pages carry per-user fields (own post, own reaction), so only their ranked ids are cached.
        var pageKey = request.UserId == null
            ? $"page:{request.Type}:{request.Platform}:{request.PageNumber}:{request.PageSize}"
            : null;

        if (pageKey != null)
        {
            var cached = await _cache.GetStringAsync(pageKey);
            if (cached != null)
            {
                return cached;
            }
        }

        var response = await GetHomeFeedWithDetail(request);
        var json = JsonSerializer.Serialize(response, _jsonOptions);

        if (pageKey != null)
        {
            await _cache.SetStringAsync(pageKey, json, AnonymousPageTtl);
        }
        return json;
    }

    /// <summary>
    /// Ranked ids of one page. The tab ranking (up to MaxCachedIds rows) is cached in Redis, so paging does not re-run
    /// Analytic / the union query, and For you keeps the same order while the user scrolls.
    /// New / Following pages past the cached window fall back to a direct DB page.
    /// </summary>
    private async Task<ListIdForHomePage> GetRankedPageAsync(PostHomeFeedR request, int pageNumber, int pageSize)
    {
        if (request.Type == HomeFeedTab.Following && request.UserId == null)
        {
            return new ListIdForHomePage { TotalItems = 0, LatestPostsResponses = new List<LatestPostsResponse>() };
        }

        var premium = request.IsPremium ? 1 : 0;
        var (key, ttl) = request.Type switch
        {
            HomeFeedTab.Following => ($"ids:following:{request.UserId}", ShortIdsTtl),
            HomeFeedTab.New => ("ids:new", ShortIdsTtl),
            HomeFeedTab.Trending => ($"ids:trending:{premium}", TrendingIdsTtl),
            _ => ($"ids:foryou:{request.UserId?.ToString() ?? "anon"}:{premium}", ForYouIdsTtl)
        };

        var ranked = await _cache.GetAsync<ListIdForHomePage>(key);
        if (ranked?.LatestPostsResponses == null)
        {
            ranked = await _socialPostService.GetHomeFeedRankedIds(request, MaxCachedIds);
            ranked.LatestPostsResponses ??= new List<LatestPostsResponse>();
            await _cache.SetAsync(key, ranked, ttl);
        }

        var offset = (pageNumber - 1) * pageSize;
        var cachedCount = ranked.LatestPostsResponses.Count;
        if (offset + pageSize <= cachedCount || cachedCount >= ranked.TotalItems)
        {
            return new ListIdForHomePage
            {
                TotalItems = ranked.TotalItems,
                LatestPostsResponses = ranked.LatestPostsResponses.Skip(offset).Take(pageSize).ToList()
            };
        }

        // Deep page of New / Following: outside the cached window
        return await _socialPostService.GetHomeFeedIds(request);
    }

    /// <summary>
    /// Hydrates ranked ids with each area's get-post-by-list-id, keeping the ranking order
    /// </summary>
    private async Task<LatestPostsDetailResponse> HydrateAsync(ListIdForHomePage ranked, PaginatedR req)
    {
        var rankedItems = ranked.LatestPostsResponses ?? new List<LatestPostsResponse>();

        // (type, hashId) -> hydrated box. HashIds are per-area random strings, so the same value could exist in two areas
        var details = new Dictionary<(PostType, string), object>();

        foreach (var group in rankedItems.Where(p => !string.IsNullOrWhiteSpace(p.HashId)).GroupBy(p => p.Type))
        {
            req.HashIds = string.Join(",", group.Select(p => p.HashId).Distinct());
            await HydrateType(group.Key, req, details);
        }

        return new LatestPostsDetailResponse
        {
            TotalItems = ranked.TotalItems,
            Items = rankedItems.Select(p => new LatestPostDetailItem
            {
                HashId = p.HashId,
                Type = p.Type,
                PostId = p.PostId,
                Point = p.Point,
                Data = string.IsNullOrEmpty(p.HashId) ? null : details.GetValueOrDefault((p.Type, p.HashId))
            }).ToList()
        };
    }

    /// <summary>
    /// Calls the owning area's "by list id" service for one PostType. A failure in one area must not blank the whole home page,
    /// so it is logged and the items of that type simply have Data = null. Cancellation is not swallowed.
    /// </summary>
    private async Task HydrateType(PostType type, PaginatedR req, Dictionary<(PostType, string), object> details)
    {
        try
        {
            switch (type)
            {
                case PostType.Feed:
                    foreach (var f in await _socialFeedService.GetFeedsByIds(req)) Put(details, type, f.HashId, f);
                    break;
                case PostType.Story:
                    foreach (var p in await _storyPostService.GetPostDetails(req)) Put(details, type, p.HashId, p);
                    break;
                case PostType.Comic:
                    foreach (var p in await _comicPostService.GetPostDetails(req)) Put(details, type, p.HashId, p);
                    break;
                case PostType.Document:
                    foreach (var p in await _documentPostService.GetPostDetails(req)) Put(details, type, p.HashId, p);
                    break;
                case PostType.TapShow:
                    // TapShow has no PaginatedR-based "by list id" query; pass the hashIds and the viewer directly
                    var tapShowIds = (req.HashIds ?? string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                    foreach (var p in await _tapShowPostService.GetByHashIdsAsync(tapShowIds, req.UserId)) Put(details, type, p.HashId, p);
                    break;
                default:
                    _logger.LogWarning("[HOME-FEED] Unsupported PostType {Type} for hashIds {HashIds}", type, req.HashIds);
                    break;
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[HOME-FEED] Hydrate {Type} failed for hashIds {HashIds}", type, req.HashIds);
        }
    }

    private static void Put(Dictionary<(PostType, string), object> details, PostType type, string? hashId, object box)
    {
        if (!string.IsNullOrEmpty(hashId))
        {
            details[(type, hashId)] = box;
        }
    }

    #endregion

    #region -- Fields --

    private readonly IPostService _socialPostService;
    private readonly IFeedService _socialFeedService;
    private readonly StoryPost _storyPostService;
    private readonly ComicPost _comicPostService;
    private readonly DocumentPost _documentPostService;
    private readonly ITapShowPostService _tapShowPostService;
    private readonly HomeFeedCache _cache;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Rows of a tab ranking kept in Redis (New / Following); deeper pages hit the DB directly
    /// </summary>
    private const int MaxCachedIds = 300;
    private static readonly TimeSpan ShortIdsTtl = TimeSpan.FromSeconds(60);
    private static readonly TimeSpan ForYouIdsTtl = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan TrendingIdsTtl = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan AnonymousPageTtl = TimeSpan.FromSeconds(60);
    private readonly ILogger<HomeFeedAggregationService> _logger;

    #endregion
}
