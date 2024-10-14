#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class PostUpdateH : BaseMinioH, IRequestHandler<PostUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="sc">Storage client</param>
    /// <param name="postService">Post service</param>
    /// <param name="metaDataService">MetaData service</param>
    /// <param name="tagService">Tag service</param>
    /// <param name="fileService">File service</param>
    /// <param name="soundService">Sound service</param>
    /// <param name="postLinkService">PostLink service</param>
    /// <param name="smartLookupService">SmartLookup service</param>
    public PostUpdateH(IMcsgContext context, ISetting setting, IStorageClient sc, IPostService postService, IMetaDataService metaDataService, ITagService tagService, IFileService fileService, ISoundService soundService, IPostLinkService postLinkService, ISmartLookupService smartLookupService, IBusinessText businessText) : base(context, setting, sc)
    {
        _postService = postService;
        _metaDataService = metaDataService;
        _tagService = tagService;
        _fileService = fileService;
        _soundService = soundService;
        _postLinkService = postLinkService;
        _smartLookupService = smartLookupService;
        _businessText = businessText;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();
        request.Tags = request.Content?.ExtractHashtags();
        request.Content = request.Content.RemoveMaliciousText();

        var vr = new PostUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(M109);
        }

        var userId = request.UserId.Value;

        #region -- Validate on server --
        // Post
        var ett = await _context.SocialPostAvailable.FirstOrDefaultAsync(p => p.HashId == request.HashId && p.Type == PostType.Feed, cancellationToken);
        if (ett == null)
        {
            throw new NotFoundException(E204, M204);
        }
        if (ett.IsDelete)
        {
            throw new BadRequestException(E205, M205);
        }
        if (ett.CreatedBy != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        if (ett.IsCompleted == true)
        {
            //throw new ForbiddenAccessException(ApiErrorCode.POST_HAS_COMPLETED, ApiErrorMessage.POST_HAS_COMPLETED);
        }
        #endregion

        var userName = request.UserName;
        var profileName = request.ProfileName;
        var profileId = request.ProfileId;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;

        var content = await _businessText.Process(request.Content);

        // Check first post
        var rewards = await _postService.CheckRewardsForPost(userId, PostType.Feed);

        ett.Update(request.Title, request.Content, request.ThumbnailUrl, request.CustomNote, userId);

        var result = new FeedPostDto
        {
            Id = ett.Id,
            Title = request.Title,
            HashId = request.HashId,
            UserId = userId,
            ThumbnailUrl = ett.ThumbnailUrl,
            CreatedOn = DateTime.UtcNow,
            Status = PostStatus.Public,
            Body = content,
            FullName = profileName,
            AuthorName = profileName,
            IsCurrentUserAuthor = true,
            ProfileId = profileId,
            UserAvatar = userAvatar,
            Rewards = rewards,
            CustomNote = ett.CustomNote
        };

        await _context.SaveChangesAsync(default);

        /*if (req.MetaData != null)
        {
            req.MetaData.Description = HttpUtility.HtmlEncode(req.MetaData.Description);
            result.MetaData = await _metaDataService.AddMetaDataToObject<Post>(req.MetaData, post.Id);
        }*/

        // Add tag to feed
        result.Tags = (await _tagService.UpdateTagsToPost(ett.Id, request.Tags, userId)).ToArray();

        // Add file to feed
        if (request.Files != null && request.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, ett.Id, ett.HashId);
            result.SubPosts = await _fileService.UpdateFilesAsync(urDto);
            result.TotalResource = result.SubPosts?.Count ?? 0;
        }
        else
        {
            await _fileService.RemoveFileAsync(ett.Id, userName);
            result.TotalResource = 0;
        }

        // Add background sound to feed
        if (request.SoundId != null && request.SoundId != Guid.Empty)
        {
            await _soundService.AddSoundAsync(ett.Id, request.SoundId.Value, userId);
        }
        else
        {
            await _soundService.RemoveSoundAsync(ett.Id);
        }

        // Detech video link content feed
        if (request.Files == null || request.Files.Count == 0)
        {
            result.Link = await _postLinkService.AddLinkAsync(ett.Id, request.Content);
        }
        else
        {
            var removeLink = await _postLinkService.RemoveLinkAsync(ett.Id);
            if (removeLink)
            {
                result.Link = new PostLinkDto();
            }
        }

        var resourceResponse = new List<ResourceDto>();
        foreach (var subPost in result.SubPosts)
        {
            if (subPost.Files != null)
            {
                foreach (var file in subPost.Files)
                {
                    var resource = new ResourceDto()
                    {
                        Body = subPost.Body,
                        Name = file?.Name ?? "",
                        HashId = file?.HashId,
                        Status = file.Status,
                        Type = file.Type,
                        Url = file?.Url ?? "",
                        Width = file.Width,
                        Height = file.Height,
                        Order = file.Order,
                        SubPostHashId = subPost.HashId,
                    };

                    resourceResponse.Add(resource);
                }
            }
        }

        result.Resources = resourceResponse;
        await _smartLookupService.CalculateSmartLookupWhenCreatePostAsync(profileName);
        result.CustomNote = result.CustomNote.ForLexical();
        res.SetSuccess(result);
        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Post service
    /// </summary>
    private readonly IPostService _postService;

    /// <summary>
    /// MetaData service
    /// </summary>
    private readonly IMetaDataService _metaDataService;

    /// <summary>
    /// Tag service
    /// </summary>
    private readonly ITagService _tagService;

    /// <summary>
    /// File service
    /// </summary>
    private readonly IFileService _fileService;

    /// <summary>
    /// Sound service
    /// </summary>
    private readonly ISoundService _soundService;

    /// <summary>
    /// PostLink service
    /// </summary>
    private readonly IPostLinkService _postLinkService;

    /// <summary>
    /// SmartLookup service
    /// </summary>
    private readonly ISmartLookupService _smartLookupService;

    /// <summary>
    /// BusinessText service
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
