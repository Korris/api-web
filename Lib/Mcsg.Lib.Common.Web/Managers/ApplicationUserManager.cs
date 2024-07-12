using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mcsg.Lib.Common.Web;

using Data.Domain.Entities;

public class ApplicationUserManager : UserManager<User>
{
    #region -- Overrides --

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

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="store"></param>
    /// <param name="optionsAccessor"></param>
    /// <param name="passwordHasher"></param>
    /// <param name="userValidators"></param>
    /// <param name="passwordValidators"></param>
    /// <param name="keyNormalizer"></param>
    /// <param name="errors"></param>
    /// <param name="services"></param>
    /// <param name="logger"></param>
    public ApplicationUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor,
        IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators,
        IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer,
        IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger) :
        base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
    {
    }

    #endregion
}
