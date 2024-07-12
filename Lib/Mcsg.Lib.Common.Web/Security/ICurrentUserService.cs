namespace Mcsg.Lib.Common.Web.Security;

using Data.Domain.Entities;
using Lib.Common.Security.Models;

public interface ICurrentUserService
{
    Task<CurrentUserModel> GetCurrentUserAsync();
    Session Session { get; }
    Task<bool> RemoveCurrentUserAsync();
}
