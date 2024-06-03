namespace Mcsg.Admin.Api.Constants
{
    public class AdminErrorMessages
    {
        //System setting
        //Global Setting
        public const string InValidGlobalSettingId = "Id {0} is invalid";
        public const string UpdateGlobalSettingExceptionMessage = "An error occurs when updating data";
        public const string GlobalSettingDoesNotExist = "Global Setting does not exist";
        //Email Setting
        public const string InValidEmailSettingId = "Id {0} is invalid";
        public const string EmailSettingDoesNotExist = "Email Setting does not exist";
        public const string UpdateEmailSettingExceptionMessage = "An error occurs when updating data";

        //User
        public const string USER_NOT_EXIST = "User does not exist";
        public const string FIRSTNAME_NOT_EMPTY = "Fistname is empty";
        public const string LASTNAME_NOT_EMPTY = "Lastname is empty";
    }
}
