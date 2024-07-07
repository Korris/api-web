using Dapper;

namespace Mcsg.Social.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Enums;
using Lib.Common.Helpers;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;

public partial class PostLinkService : IPostLinkService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<Post> _postRepository;
    private readonly IRepository<PostLink> _postLinkRepository;
    public PostLinkService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _postRepository = unitOfWork.GetRepository<Post>();
        _postLinkRepository = unitOfWork.GetRepository<PostLink>();
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
                    await _postLinkRepository.Connection.ExecuteAsync(RemoveAllLinkOfPostQuery, new { PostId = postId });
                    var postLink = new PostLink()
                    {
                        HashId = ResourcesDefinition.HashLength.GetRandomString(),
                        PostId = postId,
                        Url = lastLink.Value,
                        Type = youtubeLinks.Contains(lastLink.Value) ? PostLinkType.Youtube : PostLinkType.Video,
                        Description = ""
                    };
                    var addResult = await _postLinkRepository.InsertAsync(postLink);
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
        var result = await _postLinkRepository.Connection.ExecuteAsync(RemoveAllLinkOfPostQuery,
                                                new { PostId = postId });
        return result > 0;
    }
}
