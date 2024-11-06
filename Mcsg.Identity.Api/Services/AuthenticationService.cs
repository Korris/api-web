using Dapper;
using FluentValidation;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services;

using Analytic.Application.Protos;
using Common.Core.Constants;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork;
using Common.SeedWork.Enums;
using Common.SeedWork.Exceptions;
using Constants;
using Interfaces;
using Lib.Common.Constants;
using Lib.Data.Repositories;
using Requests;
using Response;
using Validators;
using Wallet.Api.Protos;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;
using static Constants.SocialMediaConstants;
using static SSORegister;

public partial class AuthenticationService : BaseSettingS, IAuthenticationService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="uniquenessChecker"></param>
    /// <param name="userManager"></param>
    /// <param name="tokenService"></param>
    /// <param name="userService"></param>
    /// <param name="otpService"></param>
    /// <param name="configuration"></param>
    /// <param name="logger"></param>
    /// <param name="serviceAccessor"></param>
    /// <param name="smartLookupRepository"></param>
    public AuthenticationService(IMcsgContext context, ISetting setting, IUserNameUniquenessChecker uniquenessChecker, ApplicationUserManager userManager, ITokenService tokenService, IUserService userService, IOtpService otpService, IConfiguration configuration, ILogger<AuthenticationService> logger, SSOServiceResolver serviceAccessor, IRepository<SmartLookup> smartLookupRepository) : base(context, setting)
    {
        _aes = new SecurityAes(_setting.EncryptKey);
        _uniquenessChecker = uniquenessChecker;

        _userManager = userManager;
        _tokenService = tokenService;
        _userService = userService;
        _serviceAccessor = serviceAccessor;
        _otpService = otpService;
        _smartLookupRepository = smartLookupRepository;
    }

    public async Task CheckRegisterUser(AuthenticationRegisterUserR request)
    {
        var vr = new AuthenticationRegisterUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        var user = await GetUserByEmailOrPhone(request.Email, request.Phone, true);
        if (!string.IsNullOrEmpty(request.Email) && user != null && user.EmailConfirmed == false)
        {
            throw new ForbiddenAccessException(E307, M307);
        }

        VerifyUserResponse response = new();
        if (IsAccountExisted(request.Email, request.Phone, out string code, out string message))
        {
            throw new BadRequestException(code, message);
        }

        if (!string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            var userReferrerId = await _userManager.UserAvailable.AsNoTracking()
                .Where(p => p.ReferralCode == request.ReferralCode)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
            if (userReferrerId == Guid.Empty)
            {
                var t = vr.Errors.ToValue();
                throw new BadRequestException(E116, M116);
            }
        }
    }

    public async Task<VerifyUserResponse> RegisterUser(AuthenticationRegisterUserR request)
    {
        var vr = new AuthenticationRegisterUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        if (IsAccountExisted(request.Email, request.Phone, out string code, out string message))
        {
            throw new BadRequestException(code, message);
        }

        var userReferrerId = Guid.Empty;
        if (!string.IsNullOrWhiteSpace(request.ReferralCode))
        {
            userReferrerId = await _userManager.UserAvailable.AsNoTracking()
                .Where(p => p.ReferralCode == request.ReferralCode)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
            if (userReferrerId == Guid.Empty)
            {
                var t = vr.Errors.ToValue();
                throw new BadRequestException(E116, M116);
            }
        }

        var user = await GetUserByEmailOrPhone(request.Email, request.Phone, true);
        if (user == null)
        {
            user = new User
            {
                Email = _aes.EncryptText(request.Email),
                PhoneNumber = _aes.EncryptText(request.Phone),
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                ReferralCode = _userService.GenerateReferralCode()
            };

            user.UserName = await GenerateUserName(user.Id);
            user.ProfileName = user.UserName;
            user.ProfileId = user.UserName;
            user.CreatedIp = request.RemoteIp;
            user.MinioInstance = 0; // default MinIO
            user.StorageLimit = 1024; // 1GB
            user.Type = UserType.Free;
            user.CreatedBy = request.IsForAdmin ? request.UserId : user.Id;

            var createResult = await _userManager.CreateAsync(user);
            if (!createResult.Succeeded)
            {
                var createError = createResult.Errors.FirstOrDefault();
                throw new BadRequestException(createError?.Code, createError?.Description);
            }

            if (userReferrerId != Guid.Empty)
            {
                var ettUserReferral = UserReferral.Create(userReferrerId, user.Id);
                await _context.UserReferrals.AddAsync(ettUserReferral);
                await _context.SaveChangesAsync(default);
            }

            //Init smart lookup for user
            await _smartLookupRepository.InsertAsync(new SmartLookup
            {
                CountCriteria = 0,
                Keyword = user.ProfileName,
                KeywordType = LookupKeywordType.People
            });
            await _userManager.AddToRoleAsync(user, RoleNames.User);

            _ = Task.Run(async () => await InitUserWallet(user));
            _ = Task.Run(async () => await SyncCreateToAna(user));
        }

        if (request.IsForAdmin)
        {
            await SetPassword(user, request.Password!);
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            return new VerifyUserResponse();
        }

        return await SendOtp(user);
    }

    public async Task<TokenDto> LoginUser(AuthenticationLoginUserR request)
    {
        var vr = new AuthenticationLoginUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        var user = await GetUserByEmailOrPhone(request.Email, request.Phone, false);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        // Account has been deleted
        if (user.IsDelete)
        {
            throw new ForbiddenAccessException(E305, M305);
        }

        if (!string.IsNullOrEmpty(request.Email) && user.EmailConfirmed == false)
        {
            throw new ForbiddenAccessException(E307, M307);
        }
        if (!string.IsNullOrEmpty(request.Phone) && user.PhoneNumberConfirmed == false)
        {
            throw new ForbiddenAccessException(E308, M308);
        }

        // Account has been logged into the social network
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new ForbiddenAccessException(E306, M306);
        }

        if (user.LockoutEnabled && (user.LockoutEnd == null || user.LockoutEnd >= DateTime.UtcNow))
        {
            if (user.Status == UserStatus.Suspended)
            {
                var lockoutEndFormat = user.LockoutEnd == null ? "không thời hạn" : user.LockoutEnd?.ToString();
                throw new ForbiddenAccessException(ErrorCodes.UserSuspended, string.Format(ErrorMessage.UserSuspended, lockoutEndFormat) + " - " + user.StatusReason);
            }
            else if (user.Status == UserStatus.Banned)
            {
                throw new ForbiddenAccessException(ErrorCodes.UserBanned, ErrorMessage.UserBanned + " - " + user.StatusReason);
            }
        }

        if (request.IsForAdmin && user.Type != UserType.Administrator)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }

        var signinResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (signinResult)
        {
            user.SessionId = request.SessionId;
            return await CreateAccessToken(user, request.RemoteIp);
        }
        else
        {
            throw new ForbiddenAccessException(E304, M304);
        }
    }

    public async Task<TokenDto> LoginSocial(AuthenticationLoginSocialR request)
    {
        var socialType = request.SocialType.ToLower();
        var socialMedias = new List<string> { Facebook.MediaCode, Google.MediaCode, Apple.MediaCode };
        if (string.IsNullOrEmpty(socialType) || !socialMedias.Contains(socialType))
        {
            throw new BadRequestException(ErrorCodes.SocialPlatformNotSupport, ErrorMessage.SocialPlatformNotSupport);
        }

        // Verify token
        var _ssoService = _serviceAccessor(socialType);
        var verifyTokenResponse = await _ssoService.VerifyToken(request.SocialToken) ?? throw new BadRequestException(ErrorCodes.InvalidSocialToken, ErrorMessage.SocialIdNotPublic);
        if (verifyTokenResponse.Error.Count > 0)
        {
            throw new BadRequestException(ErrorCodes.InvalidSocialToken, verifyTokenResponse.Error.FirstOrDefault());
        }

        var socialId = verifyTokenResponse.Profile.Id;
        var socialEmail = verifyTokenResponse.Profile.Email;
        var encryptedSocialId = _aes.EncryptText(socialId);
        var encryptedSocialEmail = _aes.EncryptText(socialEmail);
        var existUserId = Guid.Empty;

        // Check user exist with socialId
        var userSocial = await _context.UserSocialAvailable.FirstOrDefaultAsync(p => (p.SocialId == encryptedSocialId || p.SocialId == socialId) && p.Type == socialType);
        if (userSocial != null)
        {
            existUserId = userSocial.UserId;
        }

        // Check user exist with email
        if (existUserId == Guid.Empty)
        {
            var existUser = await _context.UserAvailable.FirstOrDefaultAsync(p => (p.Email == encryptedSocialEmail || p.Email == socialEmail));
            if (existUser != null)
            {
                existUserId = existUser.Id;
            }
        }

        // Social user linked to db. Should return access token
        if (existUserId != Guid.Empty)
        {
            var user = await _userManager.FindByIdAsync(existUserId);
            if (user == null)
            {
                throw new NotFoundException(E303, M303);
            }

            // Account has been deleted
            if (user.IsDelete)
            {
                throw new ForbiddenAccessException(E305, M305);
            }

            user.SessionId = request.SessionId;
            return await CreateAccessToken(user, request.RemoteIp);
        }
        else
        {
            // Case 1 : Can NOT GET email in social token => return error
            if (string.IsNullOrEmpty(socialEmail))
            {
                throw new BadRequestException(ErrorCodes.SocialEmailNotPublic, ErrorMessage.SocialEmailNotPublic);
            }
            else
            {
                //  Case 2 : Can GET email in social token => register
                var user = new User
                {
                    Email = _aes.EncryptText(socialEmail),
                    PhoneNumber = string.Empty,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    ReferralCode = _userService.GenerateReferralCode(),
                    ActivedDate = DateTime.UtcNow,
                };

                user.UserName = await GenerateUserName(user.Id);
                user.ProfileName = user.UserName;
                user.ProfileId = user.UserName;
                user.CreatedIp = request.RemoteIp;
                user.MinioInstance = 0; // default MinIO
                user.StorageLimit = 1024; // 1GB
                user.Type = UserType.Free;
                user.CreatedBy = user.Id;

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var createError = createResult.Errors.FirstOrDefault();
                    throw new BadRequestException(createError?.Code, createError?.Description);
                }

                //Init smart lookup for user
                await _smartLookupRepository.InsertAsync(new SmartLookup
                {
                    CountCriteria = 0,
                    Keyword = user.ProfileName,
                    KeywordType = LookupKeywordType.People
                });

                await _userManager.AddToRoleAsync(user, RoleNames.User);

                _ = Task.Run(async () => await InitUserWallet(user));
                _ = Task.Run(async () => await SyncCreateToAna(user));

                var ettUserSocial = new UserSocial
                {
                    UserId = user.Id,
                    SocialId = encryptedSocialId,
                    Type = socialType,
                    Email = encryptedSocialEmail,
                    FirstName = verifyTokenResponse.Profile.FirstName,
                    LastName = verifyTokenResponse.Profile.LastName,
                    IsRegisterBySocial = true,
                    RegisterBySocialPlatform = socialType
                };
                await _context.UserSocials.AddAsync(ettUserSocial);

                user.SessionId = request.SessionId;
                var res = await CreateAccessToken(user, request.RemoteIp);
                res.IsFirstTimeLoginBySocial = true;
                return res;
            }
        }
    }

    public async Task<bool> LogOut(BaseR request)
    {
        return await _tokenService.DeleteAsync(request.UserId ?? Guid.Empty);
    }

    public async Task<VerifyUserResponse> ResendOtp(AuthenticationResendOtpR request)
    {
        var res = new VerifyUserResponse();

        var user = await GetUserByEmailOrPhone(request.Email, request.Phone, true);
        if (user != null)
        {
            return await SendOtp(user);
        }

        var type = request.Type;
        var otpToken = request.OtpToken;

        // Case current user
        if (string.IsNullOrEmpty(otpToken))
        {
            user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException(E303, M303);
            }

            if (type == UserOtpType.VerifyEmail)
            {
                if (user.EmailConfirmed)
                {
                    throw new BadRequestException(ErrorCodes.EmailConfirmed, ErrorMessage.EmailConfirmed);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.Email, type);
                res.Token = userOtp.Token;
                res.IsEmail = true;
            }
            else if (type == UserOtpType.ResetByEmail)
            {
                if (!user.EmailConfirmed)
                {
                    throw new BadRequestException(E307, M307);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.Email, type);
                res.Token = userOtp.Token;
                res.IsEmail = true;
            }
            else if (type == UserOtpType.VerifyPhone)
            {
                if (user.PhoneNumberConfirmed)
                {
                    throw new BadRequestException(IdentityErrorCodes.MobileConfirmed, ApiMessages.MobileConfirmed);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, type);
                res.Token = userOtp.Token;
                res.IsPhone = true;
            }
            else if (type == UserOtpType.ResetByPhone)
            {
                if (!user.PhoneNumberConfirmed)
                {
                    throw new BadRequestException(E308, M308);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, type);
                res.Token = userOtp.Token;
                res.IsPhone = true;
            }
        }
        else
        {
            var otp = await _otpService.GetAsync(otpToken, type);
            if (otp != null)
            {
                var userOtp = await _otpService.CreateAsync(otp.UserId, otp.Destination, otp.OtpType);
                res.Token = userOtp.Token;
                res.IsEmail = otp.OtpType == UserOtpType.VerifyEmail;
                res.IsPhone = otp.OtpType == UserOtpType.VerifyPhone;

                // Delete
                await _context.UserOtps.Where(p => p.Id == otp.Id).ExecuteDeleteAsync();
            }
            else
            {
                throw new NotFoundException(E301, M301);
            }
        }

        return res;
    }

    public async Task<TokenDto> ChangePassword(AuthenticationChangePasswordR request)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var isCurrentPassword = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        if (!isCurrentPassword)
        {
            throw new BadRequestException(ErrorCodes.CurrentPasswordNotMatch, ErrorMessage.CurrentPasswordNotMatch);
        }

        if (request.OldPassword == request.NewPassword)
        {
            throw new BadRequestException(ErrorCodes.NewPasswordShouldDifferentCurrent, ErrorMessage.NewPasswordShouldDifferentCurrent);
        }

        if (request.NewPassword != request.ConfirmPassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        if (changePasswordResult.Succeeded)
        {
            await LogOut(request);

            user.SessionId = request.SessionId;
            return await CreateAccessToken(user, null);
        }
        else
        {
            var createError = changePasswordResult.Errors.FirstOrDefault();
            throw new BadRequestException(createError?.Code, createError?.Description);
        }
    }

    public async Task<VerifyUserResponse> ForgotPassword(AuthenticationForgotPasswordR request)
    {
        var res = new VerifyUserResponse();
        var code = "";

        var email = request.Email;
        var phone = request.Phone;
        var encryptedEmail = _aes.EncryptText(email);
        var encryptedPhone = _aes.EncryptText(phone);

        var qUser = _context.UserAvailable.AsNoTracking();
        User? user = null;

        if (!string.IsNullOrEmpty(email))
        {
            user = await qUser.FirstOrDefaultAsync(p => p.Email == encryptedEmail || p.Email == email);
            if (user == null)
            {
                throw new NotFoundException(E303, M303);
            }

            var userOtp = await _otpService.CreateAsync(user.Id, user.Email, UserOtpType.ResetByEmail);
            res.Token = userOtp.Token;
            code = userOtp.Code;
            res.IsEmail = true;
        }

        if (!string.IsNullOrEmpty(phone))
        {
            user = await qUser.FirstOrDefaultAsync(p => p.PhoneNumber == encryptedPhone || p.PhoneNumber == phone);
            if (user == null)
            {
                throw new NotFoundException(E303, M303);
            }

            var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, UserOtpType.ResetByPhone);
            res.Token = userOtp.Token;
            code = userOtp.Code;
            res.IsPhone = true;
        }

        if (_setting.DevMode)
        {
            res.Code = code;
            res.UserName = user?.UserName;
        }

        return res;
    }

    public async Task<bool> ResetPassword(AuthenticationResetPasswordR request)
    {
        var user = await GetUserByEmailOrPhone(request.Type, request.Email, request.Phone);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var valid = await _otpService.VerifyAsync(request.OtpToken, request.OtpCode, request.Type);
        if (!valid)
        {
            throw new BadRequestException(E301, M301);
        }

        if (request.Password != request.RetypePassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        var isOldPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (isOldPassword)
        {
            throw new BadRequestException(ErrorCodes.NewPasswordShouldDifferentCurrent, ErrorMessage.NewPasswordShouldDifferentCurrent);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPass = await _userManager.ResetPasswordAsync(user, code, request.Password);
        if (!resetPass.Succeeded)
        {
            var createError = resetPass.Errors.FirstOrDefault();
            throw new BadRequestException(createError?.Code, createError?.Description);
        }

        await _otpService.ClearAllUserOtpAsync(user.Id, request.Type);

        // To restore the account that has been deleted
        if (user.IsDelete)
        {
            await DeleteRestoreUserAsync(user.Id, false);

            user.Status = UserStatus.Active;
            user.IsDelete = false;
            user.DeletedAt = null;
            user.DeletedBy = null;

            await _userManager.UpdateAsync(user);
        }

        // If otp is valid, email will be confirmed and then reset password
        if (!user.EmailConfirmed)
        {
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
        }

        return resetPass.Succeeded;
    }

    public async Task<bool> CreateNewUserPassword(AuthenticationSetPasswordR request)
    {
        var otpType = !string.IsNullOrEmpty(request.Email) ? UserOtpType.VerifyEmail : UserOtpType.VerifyPhone;
        var valid = await _otpService.VerifyAsync(request.OtpToken, request.Otp, otpType);
        if (!valid)
        {
            throw new BadRequestException(E301, M301);
        }

        var user = await GetUserByEmailOrPhone(otpType, request.Email, request.Phone);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var isHasPassword = await _userManager.HasPasswordAsync(user);
        if (isHasPassword)
        {
            throw new NotFoundException(ErrorCodes.UserAlreadyHasPassword, ErrorMessage.UserAlreadyHasPassword);
        }

        if (request.Password != request.ConfirmPassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        await SetPassword(user, request.Password);

        if (otpType == UserOtpType.VerifyEmail)
        {
            user.EmailConfirmed = true;
        }

        if (otpType == UserOtpType.VerifyPhone)
        {
            user.PhoneNumberConfirmed = true;
        }

        await _userManager.UpdateAsync(user);
        await _otpService.ClearAllUserOtpAsync(user.Id, otpType);

        return true;
    }

    public async Task<RefreshTokenResponse> VerifyRefreshToken(AuthenticationRefreshTokenR request)
    {
        var userId = await _tokenService.IsValidAsync(request.RefreshToken);
        if (userId == null)
        {
            throw new UnauthorizedAccessException(E302, M302);
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        user.SessionId = request.SessionId;
        var res = await CreateAccessToken(user, null);

        return new RefreshTokenResponse
        {
            AccessToken = res.AccessToken,
            ExpiredDate = res.ExpiredDate,
            RefreshToken = res.RefreshToken,
            RefreshTokenExpiredDate = res.RefreshTokenExpiredDate
        };
    }

    public async Task<bool> DeleteUser(AuthenticationDeleteUserR request)
    {
        var vr = new AuthenticationDeleteUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(E000, t);
        }

        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        // Account has been logged into the social network
        if (string.IsNullOrEmpty(user.PasswordHash))
        {
            throw new ForbiddenAccessException(E306, M306);
        }

        var ok = await _userManager.CheckPasswordAsync(user, request.Password + "");
        if (!ok)
        {
            throw new ForbiddenAccessException(E304, M304);
        }

        await DeleteRestoreUserAsync(user.Id, true);

        user.IsDelete = true;
        user.DeletedAt = DateTime.UtcNow; // then, HostedDeleteAccount in Function.Job will update the status to UserStatus.Deleted
        user.DeletedBy = request.UserId;

        await _userManager.UpdateAsync(user);

        _ = Task.Run(async () => await SyncDeleteToAna(user.Id));

        return true;
    }

    /// <summary>
    /// Asynchronously deletes or restores a user
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <param name="isDelete">true to delete the user; false to restore the user</param>
    /// <returns>Returns the result</returns>
    /// <exception cref="BadRequestException">Thrown when the request is invalid</exception>
    private async Task DeleteRestoreUserAsync(Guid userId, bool isDelete)
    {
        try
        {
            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var postIds = await _context.ComicPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
            var now = DateTime.UtcNow;
            var sql = "CALL comic.sp_delete_restore_post_related_data(@PostId, @ModifiedBy, @ModifiedOn, @IsDelete);";
            foreach (var i in postIds)
            {
                var param = new { PostId = i, ModifiedBy = userId, ModifiedOn = now, IsDelete = isDelete };
                var data = await connection.QueryAsync(sql, param);
            }

            postIds = await _context.SocialPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
            now = DateTime.UtcNow;
            sql = "CALL social.sp_delete_restore_post_related_data(@PostId, @ModifiedBy, @ModifiedOn, @IsDelete);";
            foreach (var i in postIds)
            {
                var param = new { PostId = i, ModifiedBy = userId, ModifiedOn = now, IsDelete = isDelete };
                var data = await connection.QueryAsync(sql, param);
            }

            postIds = await _context.StoryPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
            now = DateTime.UtcNow;
            sql = "CALL story.sp_delete_restore_post_related_data(@PostId, @ModifiedBy, @ModifiedOn, @IsDelete);";
            foreach (var i in postIds)
            {
                var param = new { PostId = i, ModifiedBy = userId, ModifiedOn = now, IsDelete = isDelete };
                var data = await connection.QueryAsync(sql, param);
            }

            await connection.CloseAsync();
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.DefaultError, ex.Message);
        }
    }

    private bool IsAccountExisted(string? email, string? phone, out string code, out string message)
    {
        var res = false;

        var encryptedEmail = _aes.EncryptText(email);
        var encryptedPhone = _aes.EncryptText(phone);

        message = string.Empty;
        code = string.Empty;

        var qUser = _context.Users.Where(p => p.Status != UserStatus.Deleted).AsNoTracking();
        User? user = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            user = qUser.FirstOrDefault(p => p.Email == encryptedEmail || p.Email == email);
            if (user != null)
            {
                code = ErrorCodes.DuplicateUser;
                message = ErrorMessage.EmailExist;
            }
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            user = qUser.FirstOrDefault(p => p.PhoneNumber == encryptedPhone || p.PhoneNumber == phone);
            if (user != null)
            {
                code = ErrorCodes.DuplicateUserPhone;
                message = ErrorMessage.MobileNumberExist;
            }
        }

        if (user != null)
        {
            res = true;

            if (user.IsDelete)
            {
                code = E305;
                message = M305;
            }
        }

        return res;
    }

    private async Task<User?> GetUserByEmailOrPhone(string? email, string? phone, bool forRegister)
    {
        var encryptedEmail = _aes.EncryptText(email);
        var encryptedPhone = _aes.EncryptText(phone);

        var qUser = _context.UserAvailable;

        User? user = null;
        if (forRegister)
        {
            var qUserNameHistory = _context.UserNameHistoryAvailable.AsNoTracking();

            // Find by UserName
            user = await (from a in qUser
                          join b in qUserNameHistory on a.Id equals b.UserId
                          where !string.IsNullOrEmpty(b.UserName) && (b.UserName == encryptedEmail || b.UserName == email)
                          select a
                          ).FirstOrDefaultAsync();
        }

        // Find by Email, UserName or PhoneNumber
        if (user == null)
        {
            user = await qUser.FirstOrDefaultAsync(p => (!string.IsNullOrEmpty(p.Email) && (p.Email == encryptedEmail || p.UserName == email || p.Email == email))
                || (!string.IsNullOrEmpty(p.PhoneNumber) && (p.PhoneNumber == encryptedPhone || p.PhoneNumber == phone)));
        }

        return user;
    }

    private async Task<User?> GetUserByEmailOrPhone(UserOtpType type, string? email, string? phone)
    {
        var encryptedEmail = _aes.EncryptText(email);
        var encryptedPhone = _aes.EncryptText(phone);

        if (type == UserOtpType.VerifyEmail || type == UserOtpType.ResetByEmail)
        {
            return await _userManager.UserAvailable.FirstOrDefaultAsync(p => !string.IsNullOrEmpty(p.Email) && (p.Email == encryptedEmail || p.Email == email));
        }
        else
        {
            return await _userManager.UserAvailable.FirstOrDefaultAsync(p => !string.IsNullOrEmpty(p.PhoneNumber) && (p.PhoneNumber == encryptedPhone || p.PhoneNumber == phone));
        }
    }

    private async Task<TokenDto> CreateAccessToken(User user, string? remoteIp)
    {
        user.Roles = await _userManager.GetRolesAsync(user);
        var res = user.CreateJwt(_setting.Jwt);

        var rt = await _tokenService.AddAsync(user);
        if (rt != null)
        {
            res.RefreshToken = rt.RefreshToken + "";
            res.RefreshTokenExpiredDate = rt.RefreshTokenExpiryTime;
        }

        if (!string.IsNullOrWhiteSpace(remoteIp))
        {
            if (user.LastLoginDate != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
            }
            user.LastLoginIp = remoteIp;
            await _context.SaveChangesAsync(default);
        }

        return res;
    }

    private async Task<string?> GenerateUserName(Guid userId)
    {
        var ett = UserNameHistory.Create(_uniquenessChecker, userId);
        await _context.UserNameHistories.AddAsync(ett);
        await _context.SaveChangesAsync(default);
        return ett.UserName;
    }

    /// <summary>
    /// Send OTP via email or phone
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Returns the result</returns>
    private async Task<VerifyUserResponse> SendOtp(User user)
    {
        var res = new VerifyUserResponse();
        var code = "";

        if (!string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.PhoneNumber))
        {
            await _otpService.ClearAllUserOtpAsync(user.Id, UserOtpType.VerifyEmail);
            var userOtp = await _otpService.CreateAsync(user.Id, user.Email, UserOtpType.VerifyEmail);
            res.Token = userOtp.Token;
            code = userOtp.Code;
            res.IsEmail = true;
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.Email))
        {
            await _otpService.ClearAllUserOtpAsync(user.Id, UserOtpType.VerifyPhone);
            var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, UserOtpType.VerifyPhone);
            res.Token = userOtp.Token;
            code = userOtp.Code;
            res.IsPhone = true;
        }

        if (_setting.DevMode)
        {
            res.Code = code;
            res.UserName = user.UserName;
        }

        return res;
    }

    /// <summary>
    /// SetPassword
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="password">Password</param>
    /// <returns>Returns the result</returns>
    /// <exception cref="BadRequestException"></exception>
    private async Task SetPassword(User user, string password)
    {
        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var ir = await _userManager.ResetPasswordAsync(user, code, password);

        if (!ir.Succeeded)
        {
            var error = ir.Errors.FirstOrDefault();
            throw new BadRequestException(error?.Code + "", error?.Description + "");
        }
    }

    /// <summary>
    /// Init UserWallet
    /// </summary>
    /// <param name="user">User</param>
    /// <returns>Return the result</returns>
    private async Task<BaseRsp> InitUserWallet(User user)
    {
        var res = new BaseRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Web.Wallet!);

            var client = new UserWalletProto.UserWalletProtoClient(channel);
            var request = new UserWalletCreateReq
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileName = user.ProfileName
            };
            var rsp = await client.CreateAsync(request);
            res.Id = rsp.Id;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<UserCreateRsp> SyncCreateToAna(User ett)
    {
        var res = new UserCreateRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);
            var client = new UserProto.UserProtoClient(channel);

            var request = new UserCreateReq
            {
                Items =
                {
                    new UserProtoDto
                    {
                        UserId = ett.Id.ToString(),
                        Username = ett.ProfileName,
                        UserStatus = (int)UserStatus.Active,
                        ProfileId = ett.ProfileId,
                        CreatedOn = ett.CreatedOn.ToString(),
                        CreatedBy = ett.CreatedBy == null ? null : ett.CreatedBy.ToString(),
                        ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                        ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString()
                    }
                }
            };
            var rsp = await client.CreateAsync(request);

            res.Message = rsp.Message;
            res.Items.Add(rsp.Items.Select(item => new UserOutputDto { Id = item.Id }));
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<UserDeleteRsp> SyncDeleteToAna(Guid id)
    {
        var res = new UserDeleteRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Admin.Analytic!);
            var client = new UserProto.UserProtoClient(channel);

            var request = new UserDeleteReq
            {
                UserId = id.ToString()
            };
            var rsp = await client.DeleteAsync(request);

            res.Message = rsp.Message;
            res.Id = rsp.Id;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// SecurityAes
    /// </summary>
    private readonly ISecurityAes _aes;

    /// <summary>
    /// Uniqueness checker
    /// </summary>
    private readonly IUserNameUniquenessChecker _uniquenessChecker;

    private readonly ApplicationUserManager _userManager;
    private readonly ITokenService _tokenService;
    private readonly IUserService _userService;
    private readonly SSOServiceResolver _serviceAccessor;
    private readonly IOtpService _otpService;
    private readonly IRepository<SmartLookup> _smartLookupRepository;

    #endregion
}
