namespace Mcsg.Wallet.Api.Constants;

public class ApiErrorCodes
{
    public const string OTP_INVALID = "ERR_WAL_0001";
    public const string OTP_EXPIRED = "ERR_WAL_0002";

    public const string BALANCE_NOT_ENOUGH = "ERR_WAL_0003"; //Not enough balance
    public const string USER_NOT_FOUND = "ERR_WAL_0004";
    public const string PACKAGE_NOT_FOUND = "ERR_WAL_1004";
    public const string TRANSACTION_NOT_FOUND = "ERR_WAL_0005";
    public const string USER_NOT_PERMISSION = "ERR_WAL_0006";
    public const string WALLET_ADDRESS_NOT_FOUND = "ERR_WAL_0007";
    public const string ERROR_PAYMENT_METHOD_TYPE = "ERR_WAL_0008";
    public const string MINIMUM_CAN_DEPOSIT = "ERR_WAL_0009";  //MinimumPointCanWithDraw
    public const string CAN_NOT_TRANSFER_THEMSELEVE = "ERR_WAL_00010";
    public const string ZALO_PAY_DEPOSIT_FAIL = "ERR_WAL_00011";
    public const string ZALO_PAY_CALLBACK_FAIL = "ERR_WAL_00012";
    public const string ALREADY_PURCHARED = "ERR_WAL_0013";
    public const string USER_AS_THE_SAME_DONOR = "ERR_WAL_0014"; //Can not send donate to yourself
}
