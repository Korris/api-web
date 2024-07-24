using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Constants;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Dtos;
using Extensions;
using Interfaces;

public partial class PostLinkService : IPostLinkService
{
    public PostLinkService(McsgContext context)
    {
        _context = context;
    }

    public async Task<PostLinkDto> AddLinkAsync(Guid postId, string content)
    {
        var result = new PostLinkDto();
        var youtubeLinks = content.GetVideoLink(VideoWebsite.Youtube);
        var videoLinks = content.GetVideoLink(VideoWebsite.Video);
        var links = youtubeLinks.Concat(videoLinks).ToList();
        // Get lasted link
        if (links.Any())
        {
            var linkIndexs = new List<IntStringDto>();
            foreach (var link in links)
            {
                int indexLink = content.IndexOf(link);
                linkIndexs.Add(new IntStringDto()
                {
                    Key = indexLink,
                    Value = link
                });
            }

            if (linkIndexs != null && linkIndexs.Any())
            {
                var lastLink = linkIndexs.MaxBy(x => x.Key);
                await RemoveLinkAsync(postId);

                var postLink = new SocialPostLink
                {
                    HashId = Setting.ResourceConfig.HashLength.GetRandomString(),
                    PostId = postId,
                    Url = lastLink.Value,
                    Type = youtubeLinks.Contains(lastLink.Value) ? PostLinkType.Youtube : PostLinkType.Video,
                    Description = ""
                };
                await _context.SocialPostLinks.AddAsync(postLink);

                var addResult = await _context.SaveChangesAsync();
                if (addResult > 0)
                {
                    result.HashId = postLink.HashId;
                    result.Url = postLink.Url;
                    result.Type = postLink.Type.ToDisplay();
                }
            }
        }

        return result;
    }

    public async Task<bool> RemoveLinkAsync(Guid postId)
    {
        var postLinks = await _context.SocialPostLinkAvailable.Where(p => p.PostId == postId).ToListAsync();
        postLinks.ForEach(p => p.IsDelete = true);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgContext _context;

    #endregion
}
