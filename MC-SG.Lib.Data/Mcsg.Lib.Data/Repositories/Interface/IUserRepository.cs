using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Lib.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByPhoneAsync(string phoneNumber);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByUserNameAsync(string userName);
    }
}