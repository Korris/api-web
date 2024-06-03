using Mcsg.Lib.Common.Security.Models;
using Mcsg.Lib.Data.Domain.Entities;
using System.Threading.Tasks;

namespace Mcsg.Lib.Common.Web.Security
{
    public interface ICurrentUserService
    {
        Task<CurrentUserModel> GetCurrentUserAsync();
        Session Session { get; }
        Task<bool> RemoveCurrentUserAsync();
    }
}
