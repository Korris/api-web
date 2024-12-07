using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using NotificationType = Common.Core.Constants.Setting.NotificationType;

/// <summary>
/// Handler
/// </summary>
public class ReportViewH : BaseH, IRequestHandler<ReportViewR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public ReportViewH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(ReportViewR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new ReportViewV().Validate(request);
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

        #region -- Validate on server --
        var entityId = request.Id;
        bool isAuthor = false;

        IQueryable<BasePost> qPost;
        IQueryable<BaseReport> qReport;
        IQueryable<BaseSubPost> qSubReport;
        IQueryable<BasePostComment> qComment;

        var q = _context.Available<SocialReport>(false).Where(p => p.Id == entityId);
        SocialReport.ViewDetailDto? data = null;

        var type = request.EntityType.ToEnum(NotificationEntityType.ComicPostDelete);
        switch (request.NotificationType)
        {
            case NotificationType.LockSocial:
                data = await (from socialReport in q
                              join socialPost in _context.SocialPosts
                              on socialReport.EntityId
                              equals socialPost.Id
                              select new SocialReport.ViewDetailDto
                              {
                                  CreatedOn = socialPost.CreatedOn,
                                  AuthorId = socialPost.CreatedBy != null ? socialPost.CreatedBy.Value : Guid.Empty,
                                  ExpiredBlock = socialReport.ExpiredBlock,
                                  ReasonType = socialReport.ReasonType,
                                  ReasonText = socialReport.ReasonText,
                              })
                           .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.DeleteSocial:
                data = await _context.SocialPosts
                    .Where(p => p.Id == entityId)
                    .Select(p => new SocialReport.ViewDetailDto
                    {
                        CreatedOn = p.CreatedOn,
                        AuthorId = p.CreatedBy != null ? p.CreatedBy.Value : Guid.Empty,
                    })
                    .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.DeleteSubPost:
                if (type == NotificationEntityType.ComicSubPostDelete)
                {
                    qSubReport = _context.ComicSubPosts;
                    qPost = _context.ComicPosts;
                }
                else
                {
                    qSubReport = _context.StorySubPosts;
                    qPost = _context.StoryPosts;
                }

                data = await (from subpost in qSubReport
                              join post in qPost on subpost.PostId equals post.Id
                              where subpost.Id == entityId
                              select new SocialReport.ViewDetailDto
                              {
                                  SubPostOrder = subpost.Order,
                                  PostTitle = post.Title,
                                  AuthorId = subpost.CreatedBy ?? Guid.Empty,
                              })
                              .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.LockSubPost:
                if (type == NotificationEntityType.ComicSubPostLock)
                {
                    qReport = _context.Available<ComicReport>();
                    qSubReport = _context.ComicSubPosts;
                    qPost = _context.ComicPosts;
                }
                else
                {
                    qReport = _context.Available<StoryReport>();
                    qSubReport = _context.StorySubPosts;
                    qPost = _context.StoryPosts;
                }

                data = await (from report in qReport.Where(p => p.Id == entityId)
                              join subpost in qSubReport on report.EntityId equals subpost.Id
                              join post in qPost on subpost.PostId equals post.Id
                              select new SocialReport.ViewDetailDto
                              {
                                  SubPostOrder = subpost.Order,
                                  PostTitle = post.Title,
                                  AuthorId = subpost.CreatedBy ?? Guid.Empty,
                                  ExpiredBlock = report.ExpiredBlock,
                                  ReasonType = report.ReasonType,
                                  ReasonText = report.ReasonText,
                              })
                              .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.DeletePost:
                qPost = type == NotificationEntityType.ComicPostDelete
                    ? _context.ComicPosts
                    : _context.StoryPosts;

                data = await (from post in qPost.Where(p => p.Id == entityId)
                              select new SocialReport.ViewDetailDto
                              {
                                  PostTitle = post.Title,
                                  AuthorId = post.CreatedBy ?? Guid.Empty
                              })
                              .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.LockPost:
                if (type == NotificationEntityType.ComicPostLock)
                {
                    qReport = _context.Available<ComicReport>();
                    qPost = _context.ComicPosts;
                }
                else
                {
                    qReport = _context.Available<StoryReport>();
                    qPost = _context.StoryPosts;
                }

                data = await (from report in qReport.Where(p => p.Id == entityId)
                              join post in qPost on report.EntityId equals post.Id
                              select new SocialReport.ViewDetailDto
                              {
                                  PostTitle = post.Title,
                                  AuthorId = post.CreatedBy ?? Guid.Empty,
                                  ThumbnailUrl = post.ThumbnailUrl,
                                  CreatedOn = post.CreatedOn,
                                  ExpiredBlock = report.ExpiredBlock,
                                  ReasonType = report.ReasonType,
                                  ReasonText = report.ReasonText,
                              })
                              .FirstOrDefaultAsync(cancellationToken);
                break;

            case NotificationType.DeleteComment:
                switch (type)
                {
                    case NotificationEntityType.ComicPostCommentDelete:
                    case NotificationEntityType.ComicSubPostCommentDelete:
                        qComment = type == NotificationEntityType.ComicPostCommentDelete
                            ? _context.ComicPostComments
                            : _context.ComicSubPostComments;
                        break;

                    case NotificationEntityType.DocumentPostCommentDelete:
                    case NotificationEntityType.DocumentSubPostCommentDelete:
                        qComment = type == NotificationEntityType.DocumentPostCommentDelete
                            ? _context.DocumentPostComments
                            : _context.DocumentSubPostComments;
                        break;

                    case NotificationEntityType.SocialPostCommentDelete:
                    case NotificationEntityType.SocialSubPostCommentDelete:
                        qComment = type == NotificationEntityType.SocialPostCommentDelete
                            ? _context.SocialPostComments
                            : _context.SocialSubPostComments;
                        break;

                    default:
                        qComment = type == NotificationEntityType.StoryPostCommentDelete
                            ? _context.StoryPostComments
                            : _context.StorySubPostComments;
                        break;
                }

                data = await (from comment in qComment.Where(p => p.Id == entityId)
                              select new SocialReport.ViewDetailDto
                              {
                                  AuthorId = comment.CreatedBy ?? Guid.Empty,
                                  CreatedOn = comment.CreatedOn,
                              }).FirstOrDefaultAsync();
                break;

            default:
                break;
        }

        // Check author
        isAuthor = data?.AuthorId == request.UserId;
        if (!isAuthor)
        {
            res.SetError(nameof(E132), E132);
            return res;
        }

        #endregion

        return res.SetSuccess(data);
    }

    #endregion
}
