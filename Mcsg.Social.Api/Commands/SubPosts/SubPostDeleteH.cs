using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Dtos;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class SubPostsDeleteH : BaseH, IRequestHandler<SubPostDeleteR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public SubPostsDeleteH(IMcsgContext context) : base(context) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(SubPostDeleteR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new SubPostDeleteV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            res.SetError(E000, M000, t);
            return res;
        }

        if (request.UserId == null)
        {
            res.SetError(E109, M109);
            return res;
        }

        var userId = request.UserId.Value;

        #region -- Validate on server --
        // SubPost
        var ett = await _context.SocialSubPostAvailable.FirstOrDefaultAsync(p => p.Id == request.Id && p.UserId == userId, cancellationToken);
        if (ett == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.Id).ToCamelCase(), Value = request.Id } };
            res.SetError(E002, M002, t);
            return res;
        }
        #endregion

        if (ett.IsDelete)
        {
            res.SetError(E003, M003);
            return res;
        }

        // Delete
        ett.Delete(userId);
        await _context.SaveChangesAsync(cancellationToken);

        return res;
    }

    #endregion
}
