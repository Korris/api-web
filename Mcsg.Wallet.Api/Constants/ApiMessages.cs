namespace Mcsg.Wallet.Api.Constants;

using Common.Core.Enums;
using Domain.Enums;

public class ApiMessages
{
    public const string DONATE_TO_USER = "Msg_Wallet_Type_Donate";
    public const string TRANSFER_TO_USER = "Msg_Wallet_Type_Transer";
    public const string WITHDRAW_MESSAGE = "Msg_Wallet_Type_WithDraw";
    public const string WITHDRAW_CONTENT = "Withdraw_{0}_{1}";
    public const string DEPOSIT_MESSAGE = "Msg_Wallet_Type_Deposit";
    public const string DEPOSIT_CONTENT = "Deposit_{0}_{1}_{2}";
    public const string REWARD_FOR_NEW_USER = "Msg_Wallet_Reward_NewUser";
    public const string FROM_SYSTEM = "Msg_Wallet_From_System";
    public const string TO_SYSTEM = "Msg_Wallet_To_System";

    public static IDictionary<TransactionType, string> WALLET_TRANSACTION_TYPE
        = new Dictionary<TransactionType, string>
    {
        { TransactionType.Deposit, "Msg_Wallet_Type_Deposit" },
        { TransactionType.Withdraw, "Msg_Wallet_Type_WithDraw" },
        { TransactionType.Reward, "Msg_Wallet_Type_Reward" },
        { TransactionType.Transfer, "Msg_Wallet_Type_Transer" },
        { TransactionType.Donate, "Msg_Wallet_Type_Donate" }
    };

    public static IDictionary<TransactionStatus, string> WALLET_TRANSACTION_STATUS
       = new Dictionary<TransactionStatus, string>
   {
        { TransactionStatus.Failed, "Msg_Wallet_Status_Failed" },
        { TransactionStatus.Pending, "Msg_Wallet_Status_Pending" },
        { TransactionStatus.Success, "Msg_Wallet_Success" }
   };
    public const string ZALO_PAY_PAYMENT_ORDER = "ZaloPay - Thanh toán cho đơn hàng {0}";
}
