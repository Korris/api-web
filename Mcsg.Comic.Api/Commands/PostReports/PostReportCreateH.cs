using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Commands;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class PostReportCreateH : BaseH, IRequestHandler<PostReportCreateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public PostReportCreateH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PostReportCreateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new PostReportCreateV().Validate(request);
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
        // Check data is existed to avoid people spam
        var hasPostReport = await _context.ComicPostReports.AnyAsync(p => p.PostId == request.PostId && p.UserId == userId, cancellationToken);
        if (hasPostReport)
        {
            res.SetError(E115);
            return res;
        }
        #endregion

        // Create
        var reasonType = request.ReasonType.ToEnum(ReasonType.Backbite);
        var ett = ComicPostReport.Create(request.PostId, userId, reasonType, request.ReasonText);
        await _context.ComicPostReports.AddAsync(ett, cancellationToken);
        await _context.SaveChangesAsync(default);

        res.SetSuccess(ett.ToViewDto());

        return res;
    }

    #endregion
}