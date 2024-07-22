namespace Mcsg.Lib.Common.Web.Security;

using Lib.Common.Security.Models;
using Mcsg.Common.Domain.Entities;

public interface ICurrentUserService
{
    Task<CurrentUserModel> GetCurrentUserAsync();
    Session Session { get; }
    Task<bool> RemoveCurrentUserAsync();
}
