namespace Mcsg.Admin.Api.Constants
{
    public class AdminErrorCodes
    {
        //System setting
        public const string GlobalSettingValidate = "ERR_ADMIN_SETTING_00001";
        public const string EmailSettingValidate = "ERR_ADMIN_SETTING_00002";

        //User
        public const string USER_NOT_EXIST = "ERR_ADMIN_USER_00001";
        public const string FIRSTNAME_NOT_EMPTY = "ERR_ADMIN_USER_00002";
        public const string LASTNAME_NOT_EMPTY = "ERR_ADMIN_USER_00003";
    }
}
