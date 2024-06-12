namespace Mcsg.Wallet.Api.Constants
{
    public class ApiErrorMessage
    {
        public const string OTP_INVALID = "Otp code is invalid";
        public const string OTP_EXPIRED = "Otp is expired";
        public const string BALANCE_NOT_ENOUGH = "Not enough balance";
        public const string USER_NOT_FOUND = "User is not found";
        public const string TRANSACTION_NOT_FOUND = "Transaction not found";
        public const string USER_NOT_PERMISSION = "User not permission";
        public const string WALLET_ADDRESS_NOT_FOUND = "Wallet address is not found";
        public const string ERROR_PAYMENT_METHOD_TYPE = "System hasn't supported this payment type.";
        public const string MINIMUM_CAN_DEPOSIT = "Minimum point can deposit is ";
        public const string CAN_NOT_TRANSFER_THEMSELEVE = "User can not transfer money to themselves";
        public const string ZALO_PAY_DEPOSIT_FAIL = "Zalo pay can not create order. {0}";
        public const string ZALO_PAY_CALLBACK_FAIL = "Zalo pay callback fail";
        public const string ZALO_PAY_MAC_NOT_EQUAL = "mac not equal";
        public const string ALREADY_PURCHARED = "already purchased";
        public const string USER_AS_THE_SAME_DONOR = "User can not send donate to yourself";
    }
}
