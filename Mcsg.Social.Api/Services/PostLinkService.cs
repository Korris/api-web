using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Constants;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Enums;
using Lib.Common.Helpers;
using Lib.Data;
using Lib.Data.Domain.Entities;
using Models;

public partial class PostLinkService : IPostLinkService
{
    public PostLinkService(McsgDbContext context)
    {
        _context = context;
    }

    public async Task<PostLinkResponse> AddLinkAsync(Guid postId, string content)
    {
        try
        {
            var result = new PostLinkResponse();
            var youtubeLinks = ParserHelper.GetVideoLink(content, VideoWebsite.Youtube);
            var videoLinks = ParserHelper.GetVideoLink(content, VideoWebsite.Video);
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

                    var postLink = new PostLink
                    {
                        HashId = Setting.ResourceConfig.HashLength.GetRandomString(),
                        PostId = postId,
                        Url = lastLink.Value,
                        Type = youtubeLinks.Contains(lastLink.Value) ? PostLinkType.Youtube : PostLinkType.Video,
                        Description = ""
                    };
                    await _context.PostLinks.AddAsync(postLink);

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
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<bool> RemoveLinkAsync(Guid postId)
    {
        var postLinks = await _context.PostLinkAvailable.Where(p => p.PostId == postId).ToListAsync();
        postLinks.ForEach(p => p.IsDelete = true);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgDbContext _context;

    #endregion
}
