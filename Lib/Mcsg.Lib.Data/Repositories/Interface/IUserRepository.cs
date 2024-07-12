namespace Mcsg.Lib.Data.Repositories;

using Domain.Entities;

public interface IUserRepository : IRepository<User>
{
    Task<User> GetByPhoneAsync(string phoneNumber);

    Task<User> GetByEmailAsync(string email);

    Task<User> GetByUserNameAsync(string userName);
}