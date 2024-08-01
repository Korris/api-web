using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Mcsg.Identity.Api.Services;

using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Constants;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web;
using Lib.Common.Web.Security;
using Lib.Data.Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Requests;
using Response;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;
using static SSORegister;

public partial class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationUserManager _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IRepository<UserRefreshToken> _userRefreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserOtp> _userOtpRepository;
    private readonly ISessionService _sessionService;
    private readonly IUserService _userService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IEmailSender _sendMailService;
    private readonly SSOServiceResolver _serviceAccessor;
    private readonly IOtpService _otpService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IUserWalletService _userWalletService;
    public AuthenticationService(ApplicationUserManager userManager,
        RoleManager<Role> roleManager
        , IUnitOfWork unitOfWork
        , ISessionService sessionService
        , ITokenService tokenService
        , IPasswordHasher<User> passwordHasher
        , IUserService userService
        , ICurrentUserService currentUserService
        , IEmailSender sendMailService
        , IOtpService otpService
        , IConfiguration configuration
        , ILogger<AuthenticationService> logger
        , SSOServiceResolver serviceAccessor
        , IRepository<SmartLookup> smartLookupRepository
        , IUserWalletService userWalletService
        , IMcsgContext context
        , ISetting setting,
        IUserNameUniquenessChecker uniquenessChecker)
    {
        _userManager = userManager;
        _userRepository = unitOfWork.GetRepository<User>();
        _userOtpRepository = unitOfWork.GetRepository<UserOtp>();
        _userRefreshTokenRepository = unitOfWork.GetRepository<UserRefreshToken>();
        _sessionService = sessionService;
        _tokenService = tokenService;
        _passwordHasher = passwordHasher;
        _userService = userService;
        _currentUserService = currentUserService;
        _sendMailService = sendMailService;
        _logger = logger;
        _roleManager = roleManager;
        _serviceAccessor = serviceAccessor;
        _otpService = otpService;
        _configuration = configuration;
        _smartLookupRepository = smartLookupRepository;
        _userWalletService = userWalletService;
        _context = context;
        _setting = setting;
        _uniquenessChecker = uniquenessChecker;
    }

    public async Task<VerifyUserResponse> RegisterUser(RegisterUserReq request)
    {
        var vr = new AuthenticationRegisterUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        VerifyUserResponse response = new();
        if (IsAccountExisted(request.Email, request.Phone, out string code, out string message))
        {
            throw new BadRequestException(code, message);
        }

        var user = await GetUserByEmailOrPhoneNumber(request.Email, request.Phone);
        if (user == null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email ?? "",
                PhoneNumber = request.Phone ?? "",
                EmailConfirmed = false,
                PhoneNumberConfirmed = false,
                ReferralCode = _userService.GenerateReferralCode()
            };

            user.UserName = await GenerateUserName(user.Id);
            user.ProfileName = user.UserName;
            user.ProfileId = user.UserName;

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

            //setup wallet
            await _userWalletService.InitUserWalletAsync(user);
        }
        try
        {
            // SEND OTP VIA EMAIL/PHONE HERE
            if (!string.IsNullOrEmpty(user.Email) && string.IsNullOrEmpty(user.PhoneNumber))
            {
                await _otpService.ClearAllUserOtpAsync(user.Id, UserOtpType.VerifyEmail);
                var userOtp = await _otpService.CreateAsync(user.Id, user.Email, UserOtpType.VerifyEmail);
                response.Token = userOtp.Token;
                response.IsEmail = true;

                if (_setting.DevMode)
                {
                    response.Code = userOtp.Code;
                    response.UserName = user.UserName;
                }
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber) && string.IsNullOrEmpty(user.Email))
            {
                await _otpService.ClearAllUserOtpAsync(user.Id, UserOtpType.VerifyPhone);
                var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, UserOtpType.VerifyPhone);
                response.Token = userOtp.Token;
                response.IsPhone = true;

                if (_setting.DevMode)
                {
                    response.Code = userOtp.Code;
                    response.UserName = user.UserName;
                }
            }

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, nameof(RegisterUser), request);
            throw new BadRequestException(E500, ex.Message);
        }

    }
    public async Task<TokenDto> LoginUser(LoginUserReq request)
    {
        var vr = new AuthenticationLoginUserV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToValue();
            throw new BadRequestException(M000, t);
        }

        var user = await GetUserByEmailOrPhoneNumber(request.Email, request.Phone) ?? throw new NotFoundException(E203, M203);

        if (!request.Email.IsNullOrEmpty() && user.EmailConfirmed == false)
        {
            throw new ForbiddenAccessException(ErrorCodes.EmailNotConfirmed, string.Format(ErrorMessage.EmailNotConfirmed, request.Email));
        }
        if (!request.Phone.IsNullOrEmpty() && user.PhoneNumberConfirmed == false)
        {
            throw new ForbiddenAccessException(ErrorCodes.MobileNotConfirmed, string.Format(ErrorMessage.MobileNotConfirmed, request.Phone));
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

        var signinResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (signinResult)
        {
            var session = await _sessionService.CreateSessionAsync(user, "");
            var response = _tokenService.GenerateAccessToken(session.Id, user);
            response.Roles = session.Roles;
            response.SubscriptionKey = _configuration["Ocp-Apim-Subscription-Key"];

            var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
            if (refreshToken != null)
            {
                response.RefreshToken = refreshToken.RefreshToken;
                response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
            }

            if (user.LastLoginDate != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync(default);
            }

            return response;
        }
        else
        {
            throw new ForbiddenAccessException(ErrorCodes.PasswordInCorrect, ErrorMessage.PasswordInCorrect);
        }
    }

    public async Task<bool> LogOut()
    {
        await _sessionService.ExpireSession(_currentUserService.Session);
        return true;
    }

    public async Task<VerifyUserResponse> ResendOtp(UserOtpType type, string otpToken = "")
    {
        var verifyUserModel = new VerifyUserResponse();

        //Case current user
        if (string.IsNullOrEmpty(otpToken))
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            var user = await _userManager.FindByIdAsync(currentUser.UserId.ToString()) ?? throw new NotFoundException(E203, M203);
            if (type == UserOtpType.VerifyEmail)
            {
                if (user.EmailConfirmed)
                {
                    throw new BadRequestException(ErrorCodes.EmailConfirmed, ErrorMessage.EmailConfirmed);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.Email, type);
                verifyUserModel.Token = userOtp.Token;
                verifyUserModel.IsEmail = true;
            }
            else if (type == UserOtpType.ResetByEmail)
            {
                if (!user.EmailConfirmed)
                {
                    throw new BadRequestException(ErrorCodes.EmailNotConfirmed, string.Format(ErrorMessage.EmailNotConfirmed, user.Email));
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.Email, type);
                verifyUserModel.Token = userOtp.Token;
                verifyUserModel.IsEmail = true;
            }
            else if (type == UserOtpType.VerifyPhone)
            {
                if (user.PhoneNumberConfirmed)
                {
                    throw new BadRequestException(IdentityErrorCodes.MobileConfirmed, ApiMessages.MobileConfirmed);
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, type);
                verifyUserModel.Token = userOtp.Token;
                verifyUserModel.IsPhone = true;
            }
            else if (type == UserOtpType.ResetByPhone)
            {
                if (!user.PhoneNumberConfirmed)
                {
                    throw new BadRequestException(ErrorCodes.MobileNotConfirmed, string.Format(ErrorMessage.MobileNotConfirmed, user.PhoneNumber));
                }
                var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, type);
                verifyUserModel.Token = userOtp.Token;
                verifyUserModel.IsPhone = true;
            }
        }
        else
        {
            var otp = await _otpService.GetAsync(otpToken, type);
            if (otp != null)
            {
                var userOtp = await _otpService.CreateAsync(otp.UserId, otp.Destination, otp.OtpType);
                verifyUserModel.Token = userOtp.Token;
                verifyUserModel.IsEmail = otp.OtpType == UserOtpType.VerifyEmail;
                verifyUserModel.IsPhone = otp.OtpType == UserOtpType.VerifyPhone;

                //Clear old
                await _userOtpRepository.DeleteAsync(otp.Id);
            }
            else
            {
                throw new NotFoundException(ErrorCodes.InvalidToken, ErrorMessage.TokenInCorrect);
            }
        }
        return verifyUserModel;
    }

    public async Task<TokenDto> ChangePassword(string oldPassword, string newPassword, string confirmPassword)
    {
        var user = await _userManager.FindByIdAsync(_currentUserService.Session.UserId.ToString()) ?? throw new NotFoundException(E203, M203);

        var isCurrentPassword = await _userManager.CheckPasswordAsync(user, oldPassword);
        if (!isCurrentPassword)
        {
            throw new BadRequestException(ErrorCodes.CurrentPasswordNotMatch, ErrorMessage.CurrentPasswordNotMatch);
        }

        if (oldPassword == newPassword)
        {
            throw new BadRequestException(ErrorCodes.NewPasswordShouldDifferentCurrent, ErrorMessage.NewPasswordShouldDifferentCurrent);
        }

        if (newPassword != confirmPassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        var changePasswordResult = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

        TokenDto response;
        if (changePasswordResult.Succeeded)
        {
            await LogOut();
            await _tokenService.DeleteRefreshTokenAsync(user.Id);

            response = await CreateAccessToken(user);
        }
        else
        {
            var createError = changePasswordResult.Errors.FirstOrDefault();
            throw new BadRequestException(createError?.Code, createError?.Description);
        }
        return response;
    }

    private async Task<TokenDto> CreateAccessToken(User user)
    {
        var session = await _sessionService.CreateSessionAsync(user, "");
        TokenDto response = _tokenService.GenerateAccessToken(session.Id, user);
        response.Roles = session.Roles;
        response.SubscriptionKey = _configuration["Ocp-Apim-Subscription-Key"];

        var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
        if (refreshToken != null)
        {
            response.RefreshToken = refreshToken.RefreshToken;
            response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
        }

        return response;
    }

    public async Task<TokenDto> SetUserPassword(string password, string confirmPassword)
    {
        var response = new TokenDto();
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        var user = await _userManager.FindByIdAsync(currentUser.UserId.ToString()) ?? throw new NotFoundException(E203, M203);
        var isHasPassword = await _userManager.HasPasswordAsync(user);
        if (isHasPassword)
        {
            throw new NotFoundException(ErrorCodes.UserAlreadyHasPassword, ErrorMessage.UserAlreadyHasPassword);
        }

        if (password != confirmPassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, code, password);

        if (!resetPasswordResult.Succeeded)
        {
            var createError = resetPasswordResult.Errors.FirstOrDefault();
            throw new BadRequestException(createError?.Code, createError?.Description);
        }

        if (currentUser != null)
        {
            response = _tokenService.GenerateAccessToken(Guid.Parse(currentUser.SessionId), user);
            var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
            if (refreshToken != null)
            {
                response.RefreshToken = refreshToken.RefreshToken;
                response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
                user.RefreshToken = refreshToken.RefreshToken;
            }
        }

        return response;
    }

    public async Task<VerifyUserResponse> ForgotPassword(string email, string phone)
    {
        var response = new VerifyUserResponse();
        if (!string.IsNullOrEmpty(email))
        {
            var user = await _userManager.FindByEmailAsync(email) ?? throw new NotFoundException(E203, M203);
            var userOtp = await _otpService.CreateAsync(user.Id, user.Email, UserOtpType.ResetByEmail);
            response.Token = userOtp.Token;
            response.IsEmail = true;

        }
        else if (!string.IsNullOrEmpty(phone))
        {
            var user = await _userRepository.Connection.QueryFirstOrDefaultAsync<User>(GetUserByPhoneQuery, new { Phone = phone });
            if (user == null)
            {
                throw new NotFoundException(E203, M203);
            }
            var userOtp = await _otpService.CreateAsync(user.Id, user.PhoneNumber, UserOtpType.ResetByPhone);
            response.Token = userOtp.Token;
            response.IsPhone = true;
        }
        return response;
    }
    public async Task<bool> ResetPassword(ResetPasswordReq request)
    {
        var user = await GetUserByEmailOrPhone(request.Type, request.Email, request.Phone) ?? throw new NotFoundException(E203, M203);
        var valid = await _otpService.VerifyAsync(request.OtpToken, request.OtpCode, request.Type);
        if (!valid)
        {
            throw new BadRequestException(ErrorCodes.InvalidToken, ErrorMessage.TokenInCorrect);
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
        return resetPass.Succeeded;
    }

    public async Task<TokenDto> SocialLogin(string socialType, string socialToken)
    {
        socialType = socialType.ToLower();
        List<string> socialMedias = new() {
            SocialMediaConstants.Facebook.MediaCode
            , SocialMediaConstants.Google.MediaCode
            , SocialMediaConstants.Apple.MediaCode
        };

        if (string.IsNullOrEmpty(socialType) || !socialMedias.Contains(socialType))
        {
            throw new BadRequestException(ErrorCodes.SocialPlatformNotSupport, ErrorMessage.SocialPlatformNotSupport);
        }

        //verify token
        var _ssoService = _serviceAccessor(socialType);
        var verifyTokenResponse = await _ssoService.VerifyToken(socialToken) ?? throw new BadRequestException(ErrorCodes.InvalidSocialToken, ErrorMessage.SocialIdNotPublic);
        if (verifyTokenResponse.Error.Count > 0)
        {
            throw new BadRequestException(ErrorCodes.InvalidSocialToken, verifyTokenResponse.Error.FirstOrDefault());
        }
        var socialId = verifyTokenResponse.Profile.Id;
        var socialEmail = verifyTokenResponse.Profile.Email;

        // Check user exist ?
        var existUserId = Guid.Empty;

        // Check user exist with socialId
        var userSocial = await _ssoService.GetUserSocialBySocialId(socialType, socialId);
        if (userSocial != null)
        {
            existUserId = userSocial.UserId;
        }

        // Check user exist with email
        if (existUserId == Guid.Empty)
        {
            var existUser = await _userManager.FindByEmailAsync(socialEmail);
            if (existUser != null && !existUser.IsDelete)
            {
                existUserId = existUser.Id;
            }
        }
        // Social user linked to db. Should return access token
        if (existUserId != Guid.Empty)
        {
            var user = await _userManager.FindByIdAsync(existUserId.ToString());
            if (user == null)
            {
                throw new NotFoundException(E203, M203);
            }
            else
            {
                var session = await _sessionService.CreateSessionAsync(user, "");
                var response = _tokenService.GenerateAccessToken(session.Id, user);
                response.Roles = session.Roles;
                response.SubscriptionKey = _configuration["Ocp-Apim-Subscription-Key"];

                var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
                if (refreshToken != null)
                {
                    response.RefreshToken = refreshToken.RefreshToken;
                    response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
                }
                if (user.LastLoginDate != null)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                }
                await _userRepository.UpdateAsync(user);
                return response;
            }
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
                    Email = socialEmail,
                    PhoneNumber = string.Empty,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = false,
                    ReferralCode = _userService.GenerateReferralCode(),
                    ActivedDate = DateTime.UtcNow,
                };

                user.UserName = await GenerateUserName(user.Id);
                user.ProfileName = user.UserName;
                user.ProfileId = user.UserName;

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

                //setup wallet
                await _userWalletService.InitUserWalletAsync(user);

                var socialInfo = new UserSocial()
                {
                    UserId = user.Id,
                    SocialId = socialId,
                    Type = socialType,
                    Email = socialEmail,
                    FirstName = verifyTokenResponse.Profile.FirstName,
                    LastName = verifyTokenResponse.Profile.LastName,
                    IsRegisterBySocial = true,
                    RegisterBySocialPlatform = socialType
                };
                await _ssoService.AddUserSocial(socialInfo);

                var session = await _sessionService.CreateSessionAsync(user, "");
                var response = _tokenService.GenerateAccessToken(session.Id, user);
                response.SubscriptionKey = _configuration["Ocp-Apim-Subscription-Key"];

                var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
                if (refreshToken != null)
                {
                    response.RefreshToken = refreshToken.RefreshToken;
                    response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
                }
                if (user.LastLoginDate != null)
                {
                    user.LastLoginDate = DateTime.UtcNow;
                }
                await _userRepository.UpdateAsync(user);
                return response;
            }
        }
    }

    public async Task<TokenDto> VerifyRegisterOtp(UserOtpType type, string email, string phone, string otp, string otpToken)
    {
        var valid = await _otpService.VerifyAsync(otpToken, otp, type);
        if (valid)
        {
            //EmailConfirmed user
            var user = await GetUserByEmailOrPhone(type, email, phone) ?? throw new NotFoundException(E203, M203);
            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);
            await _otpService.ClearAllUserOtpAsync(user.Id, type);

            var sessionId = (await _sessionService.CreateSessionAsync(user, ""))?.Id.ToString();
            var response = _tokenService.GenerateAccessToken(Guid.Parse(sessionId), user);
            var refreshToken = await _tokenService.AddUserRefreshTokenAsync(user);
            if (refreshToken != null)
            {
                response.RefreshToken = refreshToken.RefreshToken;
                response.RefreshTokenExpiredDate = refreshToken.RefreshTokenExpiryTime;
            }
            return response;
        }
        throw new BadRequestException(ErrorCodes.InvalidToken, ErrorMessage.TokenInCorrect);
    }

    public async Task<bool> CreateNewUserPassword(string email, string phone, string otp, string otpToken, string password, string confirmPassword)
    {
        var otpType = !string.IsNullOrEmpty(email) ? UserOtpType.VerifyEmail : UserOtpType.VerifyPhone;
        var valid = await _otpService.VerifyAsync(otpToken, otp, otpType);
        if (!valid)
        {
            throw new BadRequestException(ErrorCodes.InvalidToken, ErrorMessage.TokenInCorrect);
        }

        //EmailConfirmed user
        var user = await GetUserByEmailOrPhone(otpType, email, phone) ?? throw new NotFoundException(E203, M203);
        var isHasPassword = await _userManager.HasPasswordAsync(user);
        if (isHasPassword)
        {
            throw new NotFoundException(ErrorCodes.UserAlreadyHasPassword, ErrorMessage.UserAlreadyHasPassword);
        }

        if (password != confirmPassword)
        {
            throw new BadRequestException(ErrorCodes.PassShouldEqualConfirmPass, ErrorMessage.PassShouldEqualConfirmPass);
        }

        var code = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, code, password);

        if (!resetPasswordResult.Succeeded)
        {
            var createError = resetPasswordResult.Errors.FirstOrDefault();
            throw new BadRequestException(createError?.Code, createError?.Description);
        }

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

    public async Task<RefreshTokenResponse> VerifyRefreshToken(string refreshToken)
    {
        var userId = await _tokenService.IsValidRefreshTokenAsync(refreshToken);
        if (userId == Guid.Empty)
        {
            throw new UnauthorizedAccessException(ErrorCodes.InvalidRefreshToken, ErrorMessage.TokenInCorrect);
        }

        var user = await _userRepository.GetByIdAsync(userId) ?? throw new NotFoundException(E203, M203);
        var userRefreshToken = await _tokenService.AddUserRefreshTokenAsync(user);

        var sessionId = (await _sessionService.CreateSessionAsync(user, ""))?.Id.ToString();

        var accessToken = _tokenService.GenerateAccessToken(Guid.Parse(sessionId), user);

        return new RefreshTokenResponse
        {
            AccessToken = accessToken.AccessToken,
            ExpiredDate = accessToken.ExpiredDate,
            RefreshToken = userRefreshToken.RefreshToken,
            SubscriptionKey = _configuration["Ocp-Apim-Subscription-Key"],
            RefreshTokenExpiredDate = userRefreshToken.RefreshTokenExpiryTime,
        };
    }

    #region Private method
    private bool IsAccountExisted(string email, string phoneNumber, out string code, out string message)
    {
        message = string.Empty;
        code = string.Empty;
        bool accountExisted = false;

        if (!string.IsNullOrEmpty(phoneNumber))
        {
            if (_userManager.Users.Any(x => x.PhoneNumber == phoneNumber && x.PhoneNumberConfirmed && !x.IsDelete))
            {
                accountExisted = true;
                code = ErrorCodes.DuplicateUserPhone;
                message = ErrorMessage.MobileNumberExist;
            }
        }

        if (!string.IsNullOrEmpty(email))
        {
            if (_userManager.Users.Any(x => x.Email == email && x.EmailConfirmed && !x.IsDelete))
            {
                accountExisted = true;
                code = ErrorCodes.DuplicateUser;
                message = ErrorMessage.EmailExist;
            }
        }
        return accountExisted;
    }

    public async Task<User?> GetUserByEmailOrPhoneNumber(string email, string phoneNumber)
    {
        var qUserAvailable = _context.Users.Where(p => !p.IsDelete);

        // Find by UserName
        var qUser = from a in qUserAvailable
                    join b in _context.UserNameHistories on a.Id equals b.UserId
                    where !string.IsNullOrEmpty(b.UserName) && b.UserName == email
                    select a;

        var user = await qUser.FirstOrDefaultAsync();

        // Find by Email or PhoneNumber
        if (user == null)
        {
            user = await qUserAvailable.FirstOrDefaultAsync(p => (!string.IsNullOrEmpty(p.Email) && p.Email == email)
                || (!string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber == phoneNumber));
        }

        return user;
    }

    private async Task<User> GetUserByEmailOrPhone(UserOtpType type, string email, string phoneNumber)
    {
        if (type == UserOtpType.VerifyEmail || type == UserOtpType.ResetByEmail)
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.Email) && x.Email == email && !x.IsDelete);
        }
        else
        {
            return await _userManager.Users.FirstOrDefaultAsync(x => !string.IsNullOrEmpty(x.PhoneNumber) && x.PhoneNumber == phoneNumber && !x.IsDelete);
        }
    }

    public async Task<bool> DeleteAccount(DeleteUserReq request)
    {
        var session = await _currentUserService.GetCurrentUserAsync();
        var user = await _userManager.FindByIdAsync(session.UserId.ToString());
        var signinResult = await _userManager.CheckPasswordAsync(user, request.Password);
        if (signinResult == false)
        {
            throw new UnauthorizedAccessException(ErrorCodes.PasswordInCorrect, ErrorMessage.PasswordInCorrect);
        }
        user.IsDelete = true;

        try
        {
            //Get all post id
            var postIds = await _userRepository.Connection.QueryAsync<Guid>(GeAllPostsIdByUser, new { UserId = user.Id });
            var now = DateTime.UtcNow;
            foreach (var postId in postIds)
            {
                await _userRepository.Connection.QueryAsync(ExecSoftDeletePost, new { PostId = postId, Date = now, UserId = user.Id });
            }
            user.Email = $"d_{user.CreatedOn.Month}{user.CreatedOn.Day}{user.CreatedOn.Hour}{user.CreatedOn.Minute}_{user.Email}";
            user.UserName = $"d_{user.CreatedOn.Month}{user.CreatedOn.Day}{user.CreatedOn.Hour}{user.CreatedOn.Minute}_{user.UserName}";
            user.PhoneNumber = $"d_{user.CreatedOn.Month}{user.CreatedOn.Day}{user.CreatedOn.Hour}{user.CreatedOn.Minute}_{user.PhoneNumber}";
            await _userManager.UpdateAsync(user);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            throw new BadRequestException(ErrorCodes.DefaultError, ErrorMessage.DefaultError);
        }


        return true;

    }

    private async Task<string?> GenerateUserName(Guid userId)
    {
        var ett = UserNameHistory.Create(_uniquenessChecker, userId);
        await _context.UserNameHistories.AddAsync(ett);
        await _context.SaveChangesAsync(default);
        return ett.UserName;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Uniqueness checker
    /// </summary>
    private readonly IUserNameUniquenessChecker _uniquenessChecker;

    #endregion
}
