namespace Mcsg.Lib.Data.Repositories;

using Mcsg.Common.Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByPhoneAsync(string phoneNumber);

    Task<User> GetByEmailAsync(string email);

    Task<User> GetByUserNameAsync(string userName);
}