using Mcsg.Lib.Data.Domain.Entities;

namespace Mcsg.Identity.Api.Services.Interfaces
{
    public interface IUserWalletService
    {
        Task InitUserWalletAsync(User user);
    }
}
