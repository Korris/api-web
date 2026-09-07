using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Protos.Services;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;

public class UserAuthenticatorService : UserAuthenticatorProto.UserAuthenticatorProtoBase
{
    #region -- Overrides --

    /// <summary>
    /// View
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserAuthenticatorViewRsp> View(UserAuthenticatorViewReq request, ServerCallContext context)
    {
        var res = new UserAuthenticatorViewRsp();

        try
        {
            var userId = new Guid(request.UserId);
            var ett = await _context.Available<UserAuthenticator>(false).Where(p => p.UserId == userId).Select(p => new UserAuthenticator
            {
                Id = p.Id,
                SecretKey = p.SecretKey,
                IsTransaction = p.IsTransaction
            }).FirstOrDefaultAsync(default);
            if (ett != null)
            {
                res.SecretKey = ett.SecretKey;
                res.IsTransaction = ett.IsTransaction;

                res.Success = true;
                res.Id = ett.Id.ToString();
            }
            else
            {
                res.Message = "UserAuthenticator not found.";
            }
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserAuthenticatorService(IMcsgContext context)
    {
        _context = context;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
