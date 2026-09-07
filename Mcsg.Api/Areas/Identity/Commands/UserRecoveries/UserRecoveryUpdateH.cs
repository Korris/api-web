using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Queries;
using Mcsg.Api.Areas.Identity.Requests;

using Commands;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Identity.Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class UserRecoveryUpdateH : BaseH, IRequestHandler<UserRecoveryUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="aes">Security aes</param>
    /// <param name="userManager">Application user manager</param>
    public UserRecoveryUpdateH(IMcsgContext context, ISecurityAes aes, ApplicationUserManager userManager) : base(context)
    {
        _aes = aes;
        _userManager = userManager;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(UserRecoveryUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserRecoveryUpdateV().Validate(request);
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
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return res.SetError(nameof(E303), E303);
        }

        if (user.IsDelete)
        {
            return res.SetError(nameof(E305), E305);
        }

        var isCorrectPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isCorrectPassword)
        {
            return res.SetError(nameof(E101), E101);
        }
        #endregion

        var currentEtts = await _context.Available<UserRecovery>().Where(p => p.UserId == userId).ToListAsync(cancellationToken);
        currentEtts.ForEach(p => p.Delete(userId));

        var etts = new List<UserRecovery>();
        var data = new List<UserRecovery.ViewDto>();

        var codes = GenerateRecoveryCodes();
        foreach (var i in codes)
        {
            var encryptedCode = _aes.EncryptText(i) + "";
            var ett = UserRecovery.Create(encryptedCode, userId);
            etts.Add(ett);
            data.Add(new UserRecovery().ToViewDto(i));
        }

        await _context.UserRecoveries.AddRangeAsync(etts, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return res.SetSuccess(data);
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    /// <summary>
    /// ApplicationUserManager
    /// </summary>
    private readonly ApplicationUserManager _userManager;

    #endregion
}
