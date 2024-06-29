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

        public string GenerateReferralCode()
        {
            var referralCode = string.Empty;
            do
            {
                referralCode = 8.GetRandomString();
            } while (_userManager.Users.FirstOrDefault(x => x.ReferralCode == referralCode) != null);

            return referralCode;
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
