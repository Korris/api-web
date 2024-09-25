using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Wallet.Api.Protos.Services;

using Common.Core.Extensions;
using Common.SeedWork.Enums;
using Common.SeedWork.Extensions;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using static Common.SeedWork.Constants.Setting;

/// <summary>
/// UserWallet service
/// </summary>
public class UserWalletService : UserWalletProto.UserWalletProtoBase
{
    #region -- Overrides --

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<BaseRsp> Create(UserWalletCreateReq request, ServerCallContext context)
    {
        var res = new BaseRsp();

        try
        {
            var walletSetting = await _context.WalletSettings.FirstOrDefaultAsync();

            var ett = new UserWallet
            {
                Address = await GennerateWalletAddress(),
                Point = 0,
                RewardPoint = 0,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                ProfileName = request.ProfileName,
                Type = UserType.Free,
                Status = UserWalletStatus.Approved,
                UserId = new Guid(request.Id),
                WalletSettingId = walletSetting!.Id,
                CreatedBy = CreatedBy.System
            };
            await _context.UserWallets.AddAsync(ett);

            res.Success = await _context.SaveChangesAsync(context.CancellationToken) > 0;
            res.Id = ett.Id.ToString();
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
    public UserWalletService(IWalletContext context)
    {
        _context = context;
    }

    private async Task<string> GennerateWalletAddress()
    {
        string address = WalletAddressLength.GetRandomString().ToLower();
        while (true)
        {
            var isExisted = await _context.UserWallets.AnyAsync(x => x.Address == address);
            if (isExisted)
            {
                address = WalletAddressLength.GetRandomString().ToLower();
            }
            else
            {
                break;
            }
        }
        return address;
    }

    private async Task<string> GennerateWalletTransactionNumber()
    {
        string number = WalletTransactionLength.GetRandomString().ToLower();
        while (true)
        {
            var isExisted = await _context.WalletTransactions.AnyAsync(x => x.ReferenceNumber == number);
            if (isExisted)
            {
                number = WalletTransactionLength.GetRandomString().ToLower();
            }
            else
            {
                break;
            }
        }
        return number;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IWalletContext _context;

    #endregion

    #region -- Constants --

    private const int WalletAddressLength = 12;
    private const int WalletTransactionLength = 12;
    private const int DefaultRewardPoint = 200000;

    #endregion
}
