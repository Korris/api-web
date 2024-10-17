using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Protos.Services;

using Common.Core.Extensions;
using Common.Domain;

public class UserService : UserProto.UserProtoBase
{
    #region -- Overrides --

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserUpdateRsp> Update(UserUpdateReq request, ServerCallContext context)
    {
        var res = new UserUpdateRsp();

        try
        {
            var id = new Guid(request.UserUid);
            var ett = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == id);
            if (ett != null)
            {
                var premiumDate = request.PremiumDate.ToDateTime();
                ett.PremiumDate = DateOnly.FromDateTime(premiumDate);

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
    /// Search
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserSearchRsp> Search(UserSearchReq request, ServerCallContext context)
    {
        var res = new UserSearchRsp();

        try
        {
            var id = request.UserUid.Split(";");
            var listUsers = await _context.UserAvailable.Where(p => id.Contains(p.Id.ToString()))
                .Select(p => new UserProtoDto
                {
                    UserId = p.Id.ToString(),
                    UserName = p.UserName,
                    ProfileName = p.ProfileName,
                    UserAvatar = p.Avatar + "",
                    Email = p.Email + "",
                }).ToListAsync();
            res.Items.AddRange(listUsers);
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
    public UserService(IMcsgContext context)
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
