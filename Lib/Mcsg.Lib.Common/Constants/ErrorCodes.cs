namespace Mcsg.Lib.Common.Constants
{
    public class ErrorCodes
    {
        //Authentication Error
        public const string InvalidUserOrPass = "ERR_AUTH_00001";
        public const string InvalidUser = "ERR_AUTH_00002";
        public const string InvalidRefreshToken = "ERR_AUTH_00004";
        public const string InvalidSession = "ERR_AUTH_00005";
        public const string ErrorCreateResetPasswordToken = "ERR_AUTH_00006";
        public const string InvalidResetPasswordToken = "ERR_AUTH_00007";
        public const string ErrorResettingPassword = "ERR_AUTH_00008";
        public const string InvalidApiKey = "ERR_AUTH_00009";
        public const string DuplicateUser = "ERR_AUTH_00010";
        public const string NotExistedUser = "ERR_AUTH_00011";
        public const string InvalidSocialToken = "ERR_AUTH_00012";
        public const string SocialEmailNotPublic = "ERR_AUTH_00013";
        public const string PasswordInCorrect = "ERR_AUTH_00014";
        public const string PassShouldEqualConfirmPass = "ERR_AUTH_00015";
        public const string InvalidToken = "ERR_AUTH_00016";
        public const string SocialPlatformNotSupport = "ERR_AUTH_00017";
        public const string EmailNotConfirmed = "ERR_AUTH_00018";
        public const string MobileNotConfirmed = "ERR_AUTH_00019";
        public const string UserIsNotAllowed = "ERR_AUTH_00020";
        public const string UserIsLockedOut = "ERR_AUTH_00021";
        public const string EmailConfirmed = "ERR_AUTH_00022";
        public const string NewPasswordShouldDifferentCurrent = "ERR_AUTH_00023";
        public const string OtpGenerateFail = "ERR_AUTH_00024";
        public const string SendEmailOtpFail = "ERR_AUTH_00025";
        public const string SendPhoneOtpFail = "ERR_AUTH_00026";
        public const string AddRoleFail = "ERR_AUTH_00027";
        // Override Identity Error
        public const string DefaultError = "ERR_AUTH_00028";
        public const string ConcurrencyFailure = "ERR_AUTH_00029";
        public const string PasswordMismatch = "ERR_AUTH_00030";
        public const string LoginAlreadyAssociated = "ERR_AUTH_00031";
        public const string InvalidUserName = "ERR_AUTH_00032";
        public const string InvalidEmail = "ERR_AUTH_00033";
        public const string DuplicateUserName = "ERR_AUTH_00034";
        public const string DuplicateEmail = "ERR_AUTH_00035";
        public const string InvalidRoleName = "ERR_AUTH_00036";
        public const string DuplicateRoleName = "ERR_AUTH_00037";
        public const string UserAlreadyHasPassword = "ERR_AUTH_00038";
        public const string UserLockoutNotEnabled = "ERR_AUTH_00039";
        public const string UserAlreadyInRole = "ERR_AUTH_00040";
        public const string UserNotInRole = "ERR_AUTH_00041";
        public const string PasswordTooShort = "ERR_AUTH_00042";
        public const string PasswordRequiresNonAlphanumeric = "ERR_AUTH_00043";
        public const string PasswordRequiresDigit = "ERR_AUTH_00044";
        public const string PasswordRequiresLower = "ERR_AUTH_00045";
        public const string PasswordRequiresUpper = "ERR_AUTH_00046";
        public const string PasswordRequiresUniqueChars = "ERR_AUTH_00047";
        public const string RecoveryCodeRedemptionFailed = "ERR_AUTH_00048";
        public const string CurrentPasswordNotMatch = "ERR_AUTH_00049";

        public const string DuplicateUserPhone = "ERR_AUTH_00050";
        public const string UserBanned = "ERR_AUTH_00051";
        public const string UserSuspended = "ERR_AUTH_00052";


        //Query Result Error
        public const string QueryNotFound = "ERR_QUERY_NOT_FOUND";
        public const string QueryEmpty = "ERR_QUERY_EMPTY";
        public const string QuerySyntaxWrong = "ERR_QUERY_SYNTAX";

        //User manager
        public const string UserIsExist = "ERR_USER_IS_EXIST";
        public const string UserEmailOrPhoneExist = "ERR_USER_EMAIL_PHONE_EXIST";
        public const string UserCantDelete = "ERR_USER_CANT_DELETE";
        public const string UserCantAddRole = "ERR_USER_CANT_ADD_ROLE";

        //Portal Api 
        public const string PortalFeedContentEmpty = "ERR_PORTAL_00001";
        public const string PortalCategoryNotFound = "ERR_PORTAL_00002";
        public const string PortalTagNameNotValid = "ERR_PORTAL_00003";
        public const string PortalTagNameNotValidLength = "ERR_PORTAL_00004";

        // MCSG.Api 
        public const string NotFoundThumbnail = "ERR_API_000001";
        public const string NotFileUpload = "ERR_API_000002";
        public const string OnlyMediaFile = "ERR_API_000005";
    }
}