namespace Mcsg.Wallet.Api.Models._3rdClass.ZaloPay.Config
{
    public static class ZaloPaySubReturnCode
    {
        public const string APPID_INVALID = "-2";
        public const string HMAC_INVALID = "-53";
        public const string TIME_INVALID = "-54";
        public const string ZPW_BALANCE_NOT_ENOUGH = "-63";
        public const string APPTRANSID_INVALID = "-92";
        public const string ORDER_NOT_EXIST = "-101";
        public const string DUPLICATE_APPS_TRANS_ID = "-68";
        public const string ILLEGAL_DATA_REQUEST = "-401";
        public const string ILLEGAL_APP_REQUEST = "-402";
        public const string ILLEGAL_SIGNATURE_REQUEST = "-403";
        public const string ILLEGAL_CLIENT_REQUEST = "-405";
        public const string LIMIT_REQUEST_REACH = "-429";
        public const string SYSTEM_ERROR = "-500";
    }
}
