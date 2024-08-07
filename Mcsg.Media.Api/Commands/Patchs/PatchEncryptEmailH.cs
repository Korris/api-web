using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Media.Api.Commands;

using Common.Domain;
using Common.SeedWork;
using Common.SeedWork.Responses;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PatchEncryptEmailH : BaseH, IRequestHandler<PatchEncryptEmailR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="aes">SecurityAes</param>
    public PatchEncryptEmailH(IMcsgContext context, ISecurityAes aes) : base(context)
    {
        _aes = aes;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PatchEncryptEmailR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var users = await _context.Users.ToListAsync();
        foreach (var i in users)
        {
            var email = _aes.DecryptText(i.Email);
            i.Email = _aes.EncryptText(email);

            var phone = _aes.DecryptText(i.PhoneNumber);
            i.PhoneNumber = _aes.EncryptText(phone);
        }

        var userSocials = await _context.UserSocials.ToListAsync();
        foreach (var i in userSocials)
        {
            var socialId = _aes.DecryptText(i.SocialId);
            i.SocialId = _aes.EncryptText(socialId);

            var email = _aes.DecryptText(i.Email);
            i.Email = _aes.EncryptText(email);

            var phone = _aes.DecryptText(i.PhoneNumber);
            i.PhoneNumber = _aes.EncryptText(phone);
        }

        await _context.SaveChangesAsync(default);

        var data = $"Update: Users {users.Count} record(s) | UserSocials {userSocials.Count} record(s)";
        res.SetSuccess(data);

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    #endregion
}
