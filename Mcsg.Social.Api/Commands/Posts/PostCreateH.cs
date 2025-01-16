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

using Grpc.Net.Client;
using MediatR;
using System.Web;

namespace Mcsg.Social.Api.Commands;

using Analytic.Application.Protos;
using Common.Core;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Dtos;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Interfaces;
using Requests;
using Validators;
using static Common.Core.GoogleSheet;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class PostCreateH : BaseMinioH, IRequestHandler<PostCreateR, SingleResponse>
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
    /// <param name="businessText">BusinessText service</param>
    /// <param name="googleSheet">Sheets service</param>
    public PostCreateH(IMcsgContext context, ISetting setting, IStorageClient sc, IPostService postService, IMetaDataService metaDataService, ITagService tagService, IFileService fileService, ISoundService soundService, IPostLinkService postLinkService, ISmartLookupService smartLookupService, IBusinessText businessText, INotificationService notificationService, GoogleSheet googleSheet) : base(context, setting, sc)
    {
        _postService = postService;
        _metaDataService = metaDataService;
        _tagService = tagService;
        _fileService = fileService;
        _soundService = soundService;
        _postLinkService = postLinkService;
        _smartLookupService = smartLookupService;
        _businessText = businessText;
        _notificationService = notificationService;
        _googleSheet = googleSheet;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        request.Tags = request.Content?.ExtractHashtags();

        var vr = new PostCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }

        var userId = request.UserId.Value;
        var userName = request.UserName;
        var profileName = request.ProfileName;
        var profileId = request.ProfileId;
        var userFolder = request.UserFolder;
        var userAvatar = request.UserAvatar;
        var receiverIds = request.Content.ToGuids();

        // Check first post
        var rewards = await _postService.CheckRewardsForPost(userId, PostType.Feed);

        // Create
        var ett = SocialPost.Create(request.Title, request.Content, request.ThumbnailUrl, profileName, request.CustomNote, userId);
        ett.BuildCustomNote(request.ShortCustomNote);
        await _context.SocialPosts.AddAsync(ett, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        request.Content = await _businessText.Process(request.Content);
        if (receiverIds.Count() > 0)
        {
            await _notificationService.AddMentionNotificationAsync(new MentionPostNotificationReq
            {
                ReceiversId = receiverIds,
                EntityType = NotificationEntityType.SocialPostMention,
                UserAvatar = userAvatar,
                UserId = userId,
                UserProfileName = profileName,
                TargetId = ett.Id,
            });
        }
        var result = new FeedPostDto
        {
            Id = ett.Id,
            Title = request.Title,
            HashId = ett.HashId,
            UserId = userId,
            ThumbnailUrl = ett.ThumbnailUrl,
            CreatedOn = DateTime.UtcNow,
            Status = PostStatus.Public,
            Body = HttpUtility.HtmlDecode(request.Content),
            FullName = profileName,
            AuthorName = profileName,
            IsCurrentUserAuthor = true,
            ProfileId = profileId,
            UserAvatar = userAvatar,
            Rewards = rewards,
            CustomNote = ett.CustomNote,
            UserName = userName
        };

        if (request.MetaData != null)
        {
            request.MetaData.Description = HttpUtility.HtmlEncode(request.MetaData.Description);
            result.MetaData = await _metaDataService.AddMetaDataToObject<SocialPost>(request.MetaData, ett.Id);
        }
        if (request.Tags != null && request.Tags.Count > 0)
        {
            result.Tags = (await _tagService.AddTagsToPost(ett.Id, request.Tags, userId)).ToArray();
        }
        if (request.Files != null && request.Files.Count > 0)
        {
            var urDto = new UploadResourceDto(request.Files, userId, userFolder, userAvatar, userName, ett.Id, ett.HashId);
            result.SubPosts = await _fileService.ProcessFilesAsync(urDto);
            result.TotalResource = result.SubPosts?.Count ?? 0;
        }

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
                        Name = file?.Name ?? "",
                        HashId = file?.HashId,
                        SubPostHashId = file?.SubPostHashId,
                        Status = file.Status,
                        Type = file.Type,
                        Url = file?.Url ?? "",
                        Width = file.Width,
                        Height = file.Height,
                        Order = file.Order
                    };

                    resourceResponse.Add(resource);
                }
            }
        }

        if (result.MetaData != null && result.SubPosts.Count == 0)
        {
            var metaData = new UploadFileDto
            {
                HashId = result.HashId,
                Url = result.MetaData.Url,
                Type = ResourceType.Youtube
            };
            result.SubPosts.Add(new SubUploadFileDto
            {
                Files = new List<UploadFileDto> { metaData }
            });
        }

        result.Resources = resourceResponse;
        await _smartLookupService.CalculateSmartLookupWhenCreatePostAsync(profileName);
        result.CustomNote = result.CustomNote.ForLexical();

        #region -- WriteDataToSheet --
        var dto = new PostSheetDto
        {
            Type = GoogleFileType.Social,
            SeriesType = PostType.Feed,
            Environment = _setting.Environment,
            Link = $"{_setting.Domain}/feed/detail?id={ett.HashId}",
            HashId = ett.HashId,
            UserName = request.UserName,
            CreatedOn = ett.CreatedOn,
            Title = ett.Body,
            Platform = request.Platform
        };
        _ = Task.Run(async () => await _googleSheet.WriteDataToSheet(dto));
        #endregion

        _ = Task.Run(async () => await SyncCreateToAna(ett));

        return res.SetSuccess(result);
    }

    private async Task<SocialCreateRsp> SyncCreateToAna(SocialPost ett)
    {
        var res = new SocialCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new SocialProto.SocialProtoClient(channel);

            var request = new SocialCreateReq
            {
                Items =
                {
                    new SocialProtoDto
                    {
                        PostId = ett.Id.ToString(),
                        HashId = ett.HashId,
                        UserId = ett.UserId.ToString(),
                        Body = ett.Body,
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy == null ? null : ett.CreatedBy.ToString(),
                        ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                        ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString()
                    }
                }
            };
            var rsp = await client.CreateAsync(request);

            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

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

    /// <summary>
    /// Notification service
    /// </summary>
    private readonly INotificationService _notificationService;

    /// <summary>
    /// Google sheet
    /// </summary>
    private readonly GoogleSheet _googleSheet;

    #endregion
}
