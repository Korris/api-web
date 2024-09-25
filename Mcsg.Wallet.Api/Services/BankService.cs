using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Wallet.Api.Services;

using Common.Core.Enums;
using Constants;
using Domain.Entities;
using Domain.Interfaces;
using Interfaces;
using Models;

public class BankService : IBankService
{
    private readonly IConfiguration _configuration;
    private readonly IWalletContext _dbContext;

    public BankService(IConfiguration configuration, IWalletContext walletDbContext)
    {
        _configuration = configuration;
        _dbContext = walletDbContext;
    }

    public async Task<IEnumerable<BankResponse>> GetBanks()
    {
        var bankListDb = await _dbContext.PaymentMethods.Where(x => x.Type == PaymentMethodType.Banking && x.IsActive)
            .AsNoTracking()
            .Select(x => new BankResponse
            {
                Id = x.Id,
                SelfId = x.SelfId,
                AllowDeposit = x.AllowDeposit,
                AllowWithdrawal = x.AllowWithdrawal,
                Bin = x.Bin,
                Code = x.Code,
                Logo = x.Logo,
                Name = x.Name,
                ShortName = x.ShortName,
                SwiftCode = x.SwiftCode,
                Type = x.Type,
            })
            .ToListAsync();

        return bankListDb;
    }
    public async Task<IEnumerable<BankFromApiResp>> SyncBanks()
    {
        using HttpClient httpClient = new HttpClient();
        string bankApi = _configuration["Bank:BankListApi"];
        HttpResponseMessage response = await httpClient.GetAsync(bankApi);
        var bankList = new List<BankFromApiResp>();
        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var o2 = JsonConvert.DeserializeObject<BankListFromApiResp>(responseBody);
            bankList = o2.Data;
        }
        var bankListDb = _dbContext.PaymentMethods.Where(x => x.Type == PaymentMethodType.Banking).ToList();

        foreach (var bank in bankList)
        {
            var bankDb = bankListDb.FirstOrDefault(x => x.SelfId == bank.Id && x.Code == bank.Code);
            if (bankDb == null)
            {
                _dbContext.PaymentMethods.Add(new PaymentMethod
                {
                    Name = bank.Name,
                    Logo = bank.Logo,
                    AllowDeposit = bank.TransferSupported,
                    AllowWithdrawal = bank.TransferSupported,
                    SelfId = bank.Id,
                    Code = bank.Code,
                    Bin = bank.Bin,
                    ShortName = bank.ShortName,
                    SwiftCode = bank.Swift_Code,
                    IsActive = true,
                    Type = PaymentMethodType.Banking
                });
            }
            else
            {
                bool isUpdate = false;
                if (bank.Name != bankDb.Name)
                {
                    bankDb.Name = bank.Name;
                    isUpdate = true;
                }
                if (bank.ShortName != bankDb.ShortName)
                {
                    bankDb.ShortName = bank.ShortName;
                    isUpdate = true;
                }
                if (bank.Bin != bankDb.Bin)
                {
                    bankDb.Bin = bank.Bin;
                    isUpdate = true;
                }
                if (bank.Logo != bankDb.Logo)
                {
                    bankDb.Logo = bank.Logo;
                    isUpdate = true;
                }
                if (isUpdate)
                {
                    _dbContext.PaymentMethods.Update(bankDb);
                }
            }
            await _dbContext.SaveChangesAsync(default);
        }

        return bankList;
    }

    public List<PayMethodResp> GetDepositMethods()
    {
        return new List<PayMethodResp> {
            new PayMethodResp {
                Name = DepositMethods.BANK,
                Title = "Direct banking",
                IsEnable = true,

            },
            new PayMethodResp {
                Name = DepositMethods.ZALO_PAY,
                Title = "Zalo Pay",
                IsEnable = true,
            },
            new PayMethodResp {
                Name = DepositMethods.GIFT_CODE,
                Title = "Giftcode",
                IsEnable = true,
            }
        };
    }
    public List<PayMethodResp> GetPayMethods()
    {
        return new List<PayMethodResp> {
            new PayMethodResp {
                Name = PayMethods.POINT,
                Title = "Point",
                IsEnable = true,
            },
            new PayMethodResp {
                Name = PayMethods.BANK,
                Title = "Direct banking",
                IsEnable = true,
            },
            new PayMethodResp {
                Name = PayMethods.ZALO_PAY,
                Title = "Zalo Pay",
                IsEnable = true,
            }
        };
    }
    public string GetPayMethodTitle(string name)
    {
        var paymethods = GetPayMethods();
        return paymethods.FirstOrDefault(x => x.Name == name)?.Title;
    }
    public List<CurrencyTypeRatio> GetCurrencyTypeRatios()
    {
        return new List<CurrencyTypeRatio> {
            new CurrencyTypeRatio {
            Type = CurrencyType.VND,
            Ratio = 1
        } };
    }
}
