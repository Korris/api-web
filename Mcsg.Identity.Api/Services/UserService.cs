using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Services
{
    using Common.SeedWork.Extensions;
    using Interface;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Repositories;

    public partial class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;
        public UserService(UserManager<User> userManager,
            IRepository<User> userRepository,
            ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public async Task<string> GenerateProfileName(string email, string phone)
        {
            string profileName = string.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                profileName = email.Split('@')[0];
            }
            else if (!string.IsNullOrEmpty(phone))
            {
                var totalUser = await _userManager.Users.Select(x => x.Id).CountAsync() + 1;
                profileName = $"user-{totalUser}";
            }
            return profileName;
        }

        public string GenerateReferralCode()
        {
            var referralCode = string.Empty;
            do
            {
                referralCode = 8.GetRandomString();
            } while (_userManager.Users.FirstOrDefault(x => x.ReferralCode == referralCode) != null);

            return referralCode;
        }

        public string GenerateUserName(string email, string phone)
        {
            string userName = string.Empty;
            if (!string.IsNullOrEmpty(email))
            {
                userName = email;
            }
            else if (!string.IsNullOrEmpty(phone))
            {
                userName = phone;
            }
            return userName;
        }

        public async Task<bool> ConfirmEmailAsync(string email)
        {
            var iResult = await _userRepository.Connection.ExecuteAsync(UpdateEmailConfirmedCommand, new
            {
                email,
                id = _currentUserService.Session.UserId
            });

            return iResult > 0;
        }

        public async Task<bool> ConfirmPhoneNumberAsync(string phone)
        {
            var iResult = await _userRepository.Connection.ExecuteAsync(UpdatePhoneNumberConfirmedCommand, new
            {
                phone,
                id = _currentUserService.Session.UserId
            });

            return iResult > 0;
        }
    }
}
