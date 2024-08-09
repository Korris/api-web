using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Interfaces;

public class DeleteAccountService : IDeleteAccountService
{
    public DeleteAccountService(IMcsgContext context)
    {
        _context = context;
    }

    public async Task Run()
    {
        try
        {
            var users = await _context.Users.Where(p => p.IsDelete && p.Status != UserStatus.Deleted && p.DeletedAt < DateTime.UtcNow).Take(3).ToListAsync();
            if (users.Count == 0)
            {
                return;
            }

            foreach (var i in users)
            {
                var prefix = $"d_{i.CreatedOn.Month}{i.CreatedOn.Day}{i.CreatedOn.Hour}{i.CreatedOn.Minute}";

                var email = i.Email + "";
                var userName = i.UserName + "";
                var phone = i.PhoneNumber + "";

                email = email.Replace(prefix, "");
                userName = userName.Replace(prefix, "");
                phone = phone.Replace(prefix, "");

                i.Email = $"{prefix}_{email}";
                i.UserName = $"{prefix}_{userName}";
                i.PhoneNumber = $"{prefix}_{phone}";

                i.NormalizedEmail = i.Email.ToUpper();
                i.NormalizedUserName = i.UserName.ToUpper();

                i.Status = UserStatus.Deleted;
            }

            await _context.SaveChangesAsync(default);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
