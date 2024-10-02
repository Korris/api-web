using Microsoft.AspNetCore.Identity;

namespace Mcsg.OpenId.Mvc.Helpers;

using Lib.Common.Constants;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public class LocalizeIdentityErrorDescriber : IdentityErrorDescriber
{
    public override IdentityError DuplicateEmail(string email)
    {
        return new IdentityError()
        {
            Code = ErrorCodes.DuplicateEmail,
            Description = string.Format(ErrorMessage.DuplicateEmail, email)
        };
    }
    public override IdentityError DefaultError() { return new IdentityError { Code = ErrorCodes.DefaultError, Description = ErrorMessage.DefaultError }; }
    public override IdentityError ConcurrencyFailure() { return new IdentityError { Code = ErrorCodes.ConcurrencyFailure, Description = ErrorMessage.ConcurrencyFailure }; }
    public override IdentityError PasswordMismatch() { return new IdentityError { Code = ErrorCodes.PasswordMismatch, Description = ErrorMessage.PasswordMismatch }; }
    public override IdentityError InvalidToken() { return new IdentityError { Code = E301, Description = M301 }; }
    public override IdentityError LoginAlreadyAssociated() { return new IdentityError { Code = ErrorCodes.LoginAlreadyAssociated, Description = ErrorMessage.LoginAlreadyAssociated }; }
    public override IdentityError InvalidUserName(string userName) { return new IdentityError { Code = ErrorCodes.InvalidUserName, Description = string.Format(ErrorMessage.InvalidUserName, userName) }; }
    public override IdentityError InvalidEmail(string email) { return new IdentityError { Code = ErrorCodes.InvalidEmail, Description = string.Format(ErrorMessage.InvalidEmail, email) }; }
    public override IdentityError DuplicateUserName(string userName) { return new IdentityError { Code = ErrorCodes.DuplicateUserName, Description = string.Format(ErrorMessage.DuplicateUserName, userName) }; }
    //public override IdentityError DuplicateEmail(string email) { return new IdentityError { Code = ErrorCodes.DuplicateEmail, Description = string.Format(ErrorMessage.DuplicateEmail , email) }; }
    public override IdentityError InvalidRoleName(string role) { return new IdentityError { Code = ErrorCodes.InvalidRoleName, Description = string.Format(ErrorMessage.InvalidRoleName, role) }; }
    public override IdentityError DuplicateRoleName(string role) { return new IdentityError { Code = ErrorCodes.DuplicateRoleName, Description = string.Format(ErrorMessage.DuplicateRoleName, role) }; }
    public override IdentityError UserAlreadyHasPassword() { return new IdentityError { Code = ErrorCodes.UserAlreadyHasPassword, Description = ErrorMessage.UserAlreadyHasPassword }; }
    public override IdentityError UserLockoutNotEnabled() { return new IdentityError { Code = ErrorCodes.UserLockoutNotEnabled, Description = ErrorMessage.UserLockoutNotEnabled }; }
    public override IdentityError UserAlreadyInRole(string role) { return new IdentityError { Code = ErrorCodes.UserAlreadyInRole, Description = string.Format(ErrorMessage.UserAlreadyInRole, role) }; }
    public override IdentityError UserNotInRole(string role) { return new IdentityError { Code = ErrorCodes.UserNotInRole, Description = string.Format(ErrorMessage.UserNotInRole, role) }; }
    public override IdentityError PasswordTooShort(int length) { return new IdentityError { Code = ErrorCodes.PasswordTooShort, Description = string.Format(ErrorMessage.PasswordTooShort, length) }; }
    public override IdentityError PasswordRequiresNonAlphanumeric() { return new IdentityError { Code = ErrorCodes.PasswordRequiresNonAlphanumeric, Description = ErrorMessage.PasswordRequiresNonAlphanumeric }; }
    public override IdentityError PasswordRequiresDigit() { return new IdentityError { Code = ErrorCodes.PasswordRequiresDigit, Description = ErrorMessage.PasswordRequiresDigit }; }
    public override IdentityError PasswordRequiresLower() { return new IdentityError { Code = ErrorCodes.PasswordRequiresLower, Description = ErrorMessage.PasswordRequiresLower }; }
    public override IdentityError PasswordRequiresUpper() { return new IdentityError { Code = ErrorCodes.PasswordRequiresUpper, Description = ErrorMessage.PasswordRequiresUpper }; }
    public override IdentityError PasswordRequiresUniqueChars(int uniqueChars)
    {
        return base.PasswordRequiresUniqueChars(uniqueChars);
    }
    public override IdentityError RecoveryCodeRedemptionFailed()
    {
        return base.RecoveryCodeRedemptionFailed();
    }
}
