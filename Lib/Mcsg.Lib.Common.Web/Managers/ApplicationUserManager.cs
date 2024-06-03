using Mcsg.Lib.Data.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mcsg.Lib.Common.Web
{
    public class ApplicationUserManager : UserManager<User>
    {
        public ApplicationUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
            IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) :
            base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {

        }

        public override async Task<User> FindByEmailAsync(string email)
        {
            // Implement your custom logic here
            // For example, filter out users with IsDeleted = false
            return await Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDelete);
        }
        public override async Task<User> FindByIdAsync(string id)
        {
            var idGuid = Guid.Parse(id);
            return await Users.FirstOrDefaultAsync(u => u.Id == idGuid && !u.IsDelete);
        }
    }
}
