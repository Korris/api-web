using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Mcsg.Common.Domain;

using Core.Enums;
using Domain.Entities;

public class ApplicationUserManager : UserManager<User>
{
    #region -- Overrides --

    /// <summary>
    /// FindByIdAsync
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns></returns>
    public override async Task<User?> FindByIdAsync(string userId)
    {
        return await UserAvailable.FirstOrDefaultAsync(p => p.Id == Guid.Parse(userId));
    }

    /// <summary>
    /// FindByEmailAsync
    /// </summary>
    /// <param name="email">Email or UserName or PhoneNumber</param>
    /// <returns>Returns the user</returns>
    public override async Task<User?> FindByEmailAsync(string email)
    {
        return await UserAvailable.FirstOrDefaultAsync(p =>
            (!string.IsNullOrEmpty(p.Email) && p.Email == email) ||
            (!string.IsNullOrEmpty(p.UserName) && p.UserName == email) ||
            (!string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber == email)
        );
    }

    /// <summary>
    /// FindByNameAsync
    /// </summary>
    /// <param name="userName">Email or UserName or PhoneNumber</param>
    /// <returns>Returns the user</returns>
    public override async Task<User?> FindByNameAsync(string userName)
    {
        return await UserAvailable.FirstOrDefaultAsync(p =>
            (!string.IsNullOrEmpty(p.Email) && p.Email == userName) ||
            (!string.IsNullOrEmpty(p.UserName) && p.UserName == userName) ||
            (!string.IsNullOrEmpty(p.PhoneNumber) && p.PhoneNumber == userName)
        );
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// FindByIdAsync
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Returns the user</returns>
    public async Task<User?> FindByIdAsync(Guid? id)
    {
        return await UserAvailable.FirstOrDefaultAsync(p => p.Id == id);
    }

    #endregion

    #region -- Properties --

    public IQueryable<User> UserAvailable => Users.Where(p => p.Status != UserStatus.WillDelete && p.Status != UserStatus.Deleted);

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
