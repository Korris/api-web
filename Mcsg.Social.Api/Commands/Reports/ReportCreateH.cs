using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class ReportCreateH : BaseH, IRequestHandler<ReportCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public ReportCreateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(ReportCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new ReportCreateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(nameof(E000), t);
        }

        if (request.UserId == null)
        {
            throw new BadRequestException(nameof(E109));
        }

        var userId = request.UserId.Value;

        #region -- Validate on server --
        _ = Guid.TryParse(request.EntityId, out Guid entityId);
        var type = request.EntityType.ToEnum(EntityType.Post);
        bool isAuthor = false;

        switch (type)
        {
            case EntityType.Post:
                var post = await _context.SocialPostAvailable.FirstOrDefaultAsync(p => p.Id == entityId, cancellationToken);
                if (post == null)
                {
                    var t = new List<DicDto> { new() { Key = nameof(request.EntityId).ToCamelCase(), Value = entityId } };
                    return res.SetError(nameof(E002), E002, t);
                }

                isAuthor = post.CreatedBy == userId;
                break;

            case EntityType.SubPost:
                var subPost = await _context.SocialSubPostAvailable.FirstOrDefaultAsync(p => p.Id == entityId, cancellationToken);
                if (subPost == null)
                {
                    var t = new List<DicDto> { new() { Key = nameof(request.EntityId).ToCamelCase(), Value = entityId } };
                    return res.SetError(nameof(E002), E002, t);
                }

                isAuthor = subPost.CreatedBy == userId;
                break;

            case EntityType.CommentPost:
                var commentPost = await _context.SocialPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == entityId, cancellationToken);
                if (commentPost == null)
                {
                    var t = new List<DicDto> { new() { Key = nameof(request.EntityId).ToCamelCase(), Value = entityId } };
                    return res.SetError(nameof(E002), E002, t);
                }

                isAuthor = commentPost.CreatedBy == userId;
                break;

            case EntityType.CommentSubPost:
                var commentSubPost = await _context.SocialSubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == entityId, cancellationToken);
                if (commentSubPost == null)
                {
                    var t = new List<DicDto> { new() { Key = nameof(request.EntityId).ToCamelCase(), Value = entityId } };
                    return res.SetError(nameof(E002), E002, t);
                }

                isAuthor = commentSubPost.CreatedBy == userId;
                break;

            default:
                break;
        }

        // Check author
        if (isAuthor)
        {
            return res.SetError(nameof(E130), E130);
        }

        // Check data is existed to avoid people spam
        var ett = await _context.SocialReportAvailable.FirstOrDefaultAsync(p => p.EntityId == entityId, cancellationToken);
        if (ett != null)
        {
            var hasDetail = await _context.SocialReportDetailAvailable.AnyAsync(p => p.ReportId == ett.Id && p.UserId == userId, cancellationToken);
            if (hasDetail && ett.Status == ReportStatus.Reviewing)
            {
                return res.SetError(nameof(E115), E115);
            }

            if (ett.Status == ReportStatus.Approved)
            {
                return res.SetError(nameof(E129), E129);
            }
        }
        #endregion

        if (ett == null)
        {
            // Create Report
            ett = SocialReport.Create(entityId, type, userId);
            await _context.SocialReports.AddAsync(ett, cancellationToken);
        }

        // Create ReportDetail
        var reasonType = request.ReasonType.ToEnum(ReasonType.Other);
        var ettDetail = SocialReportDetail.Create(ett.Id, reasonType, request.ReasonText, userId);
        await _context.SocialReportDetails.AddAsync(ettDetail, cancellationToken);

        await _context.SaveChangesAsync(default);

        // Update Status
        await _context.SocialReports.Where(p => p.Id == ett.Id).ExecuteUpdateAsync(p => p.SetProperty(q => q.Status, ReportStatus.Reviewing), cancellationToken);

        return res.SetSuccess(ett.ToViewDto(ettDetail.ReasonType, ettDetail.ReasonText));
    }

    #endregion
}
