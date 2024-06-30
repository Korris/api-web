namespace Mcsg.Identity.Api.Interfaces
{
    using Lib.Data.Domain.Entities;

    public interface IUserWalletService
    {
        Task InitUserWalletAsync(User user);
    }
}
