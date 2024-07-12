namespace Mcsg.Lib.Common.Constants;

public static class ErrorMessage
{
    public const string RegisteredPhoneNumber = "Số điện thoại đã được đăng ký. Hãy thử số điện thoại khác.";
    public const string MobileNumberExist = "Số điện thoại đã tồn tại";
    public const string EmailAndMobileNumberExist = "Số điện thoại và Email đã tồn tại";
    public const string EmailExist = "Email đã tồn tại";
    public const string EmailNotConfirmed = "Email {0} chưa được xác thực";
    public const string MobileNotConfirmed = "Số điện thoại {0} chưa được xác thực";
    public const string NewPasswordShouldDifferentCurrent = "Mật khẩu mới phải khác mật khẩu cũ.";
    public const string AccountNotAllowed = "Tài khoản của bạn chưa được xác thực";
    public const string AccountLockedOut = "Tài khoản đang bị khóa";
    public const string PasswordInCorrect = "Mật khẩu không đúng";
    public const string EmailConfirmed = "Tài khoản đã xác nhận email trước đó";
    public const string PassShouldEqualConfirmPass = "Xác nhận mật khẩu không đúng";
    public const string SocialIdNotPublic = "Tài khoản mạng xã hội của bạn không công khai";
    public const string TokenNotFound = "Không tìm thấy OTP token này {0} hoặc sai loại";
    public const string TokenInCorrect = "Token hoặc OTP không đúng";
    public const string OtpGenerateFail = "Không thể tạo mã OTP";
    public const string SocialEmailNotPublic = "Email mạng xã hội của bạn không được công khai";
    public const string SocialPlatformNotSupport = "Hệ thống chưa hỗ trợ mạng xã hội này";
    public const string UserBanned = "Tài khoản của bạn đã bị khóa vĩnh viễn";
    public const string UserSuspended = "Tài khoản của bạn bị khóa đến {0}";

    // Override Identity Error
    public const string DefaultError = "Có lỗi xảy ra, vui lòng thử lại.";
    public const string ConcurrencyFailure = "Optimistic concurrency failure, object has been modified.";
    public const string PasswordMismatch = "Mật khẩu không đúng";
    public const string LoginAlreadyAssociated = "Một người dùng khác đã sử dụng tài khoản này để đăng nhập rồi.";
    public const string InvalidUserName = "Tên đăng nhập {0} không hợp lệ, tên đăng nhập chỉ được bao gồm chữ và số.";
    public const string InvalidEmail = "Email {0} không hợp lệ.";
    public const string DuplicateUserName = "Tên đăng nhập {0} đã có người sử dụng.";
    public const string DuplicateEmail = "Email {0} đã có người sử dụng.";
    public const string InvalidRoleName = "Role name {0} is invalid.";
    public const string DuplicateRoleName = "Role name {0} is already taken.";
    public const string UserAlreadyHasPassword = "User already has a password set.";
    public const string UserLockoutNotEnabled = "Lockout is not enabled for this user.";
    public const string UserAlreadyInRole = "User already in role {0}.";
    public const string UserNotInRole = "User is not in role {0}.";
    public const string PasswordTooShort = "Mật khẩu phải chứa ít nhất {0} ký tự.";
    public const string PasswordRequiresNonAlphanumeric = "Mật khẩu phải chứa ít nhất 1 ký tự đặc biệt.";
    public const string PasswordRequiresDigit = "mật khẩu phải chứa ít nhất 1 chữ số ('0'-'9').";
    public const string PasswordRequiresLower = "Mật khẩu phải chứa ít nhất 1 ký tự viết thường ('a'-'z').";
    public const string PasswordRequiresUpper = "Mật khẩu phải chứa ít nhất 1 ký tự viết hoa ('A'-'Z').";
    public const string PasswordRequiresUniqueChars = "Password requires unique chars";
    public const string RecoveryCodeRedemptionFailed = "Recovery code redemption failed";
    public const string CurrentPasswordNotMatch = "Current Password not match";

    //User manager
    public const string UserNotFound = "Không tìm thấy user này";
    public const string UserCantUpdate = "Không thể update user này";
    public const string UserCantDelete = "Không thể xóa user này";
    public const string UserHigherRole = "User bạn update có quyền cao hơn bạn";

    //Portal Api 
    public const string FeedContentEmpty = "Không được để trống nội dung";
    public const string FeedCategoryNotFound = "Không tìm thấy category id này {0}";
    public const string TagNameNotValid = "Tagname {0} not valid";
    public const string TagNameNotValidLength = "Tagname {0} not valid. Tag length must be 32 or less";

    // MCSG.Api 
    public const string NotFoundThumbnail = "No thumbnail found for the URL.";
    public const string NotFileUpload = "No file uploaded.";
    public const string OnlyMediaFile = "Only media files are allowed.";
}
