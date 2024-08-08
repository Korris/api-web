namespace Mcsg.Identity.Api.Interfaces;

using Common.Domain.Entities;

public interface IUserWalletService
{
    Task InitUserWalletAsync(User user, bool createWalletTransaction);
}
