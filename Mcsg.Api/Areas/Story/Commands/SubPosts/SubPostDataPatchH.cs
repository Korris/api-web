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

namespace Mcsg.Api.Areas.Story.Commands;
using Mcsg.Api.Interfaces;

using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Requests;
using Mcsg.Api.Areas.Story.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class SubPostDataPatchH : BaseSettingH, IRequestHandler<SubPostDataPatchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public SubPostDataPatchH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(SubPostDataPatchR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new SubPostDataPatchV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var body = "{\"type\":";
        if (request.IsLexicalToTiptap)
        {
            body = "{\"root\":";
        }

        var etts = await _context.StorySubPosts.Where(p => p.Body != null && p.Body.StartsWith(body)).ToListAsync(cancellationToken);

        foreach (var i in etts)
        {
            i.Body = request.IsLexicalToTiptap ? i.Body.ConvertLexicalToTiptap() : i.Body.ConvertTiptapToLexical();
        }

        var data = await _context.SaveChangesAsync(cancellationToken);

        return res.SetSuccess(data);
    }

    #endregion
}
