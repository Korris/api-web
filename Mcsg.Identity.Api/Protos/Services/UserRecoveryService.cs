using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Protos.Services;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;

public class UserRecoveryService : UserRecoveryProto.UserRecoveryProtoBase
{
    #region -- Overrides --

    /// <summary>
    /// View
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserRecoveryViewRsp> View(UserRecoveryViewReq request, ServerCallContext context)
    {
        var res = new UserRecoveryViewRsp();

        try
        {
            var userId = new Guid(request.UserId);
            var encryptedCode = _aes.EncryptText(request.OtpCode);
            var ett = await _context.Available<UserRecovery>(false).Where(p => p.SecretKey == encryptedCode && p.UserId == userId).
            Select(p => new UserRecovery
            {
                Id = p.Id,
                SecretKey = p.SecretKey,
                ModifiedOn = p.ModifiedOn,
            }).FirstOrDefaultAsync(default);

            if (ett != null)
            {
                res.SecretKey = ett.SecretKey;

                if (ett.ModifiedOn.HasValue)
                {
                    res.ModifiedOn = Timestamp.FromDateTime(ett.ModifiedOn.Value.ToUniversalTime());
                }

                res.Success = true;
                res.Id = ett.Id.ToString();
            }
            else
            {
                res.Message = "UserRecovery not found.";
            }
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserRecoveryUpdateRsp> Update(UserRecoveryUpdateReq request, ServerCallContext context)
    {
        var res = new UserRecoveryUpdateRsp();

        try
        {
            var userId = new Guid(request.UserId);
            var encryptedCode = _aes.EncryptText(request.OtpCode);
            var ett = await _context.Available<UserRecovery>().FirstOrDefaultAsync(p => p.SecretKey == encryptedCode && p.UserId == userId);
            if (ett != null)
            {
                ett.ModifiedBy = userId;
                ett.ModifiedOn = DateTime.UtcNow;

                res.Success = await _context.SaveChangesAsync(context.CancellationToken) > 0;
                res.Id = ett.Id.ToString();
            }
            else
            {
                res.Message = "User not found.";
            }
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserRecoveryCheckRsp> Check(UserRecoveryCheckReq request, ServerCallContext context)
    {
        var res = new UserRecoveryCheckRsp();

        try
        {
            var userId = new Guid(request.UserId);
            res.HasRecovery = await _context.Available<UserRecovery>(false).AnyAsync(p => p.UserId == userId && p.ModifiedOn == null);
            res.Success = true;
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
    public UserRecoveryService(IMcsgContext context, ISecurityAes aes)
    {
        _context = context;
        _aes = aes;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;
    #endregion
}
