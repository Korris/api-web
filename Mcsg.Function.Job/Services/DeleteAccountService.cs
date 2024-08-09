using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

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
            var users = await _context.UserAvailable.Where(p => p.DeletedAt < DateTime.UtcNow).Take(3).ToListAsync();
            if (users.Count == 0)
            {
                return;
            }

            foreach (var user in users)
            {
                var prefix = $"d_{user.CreatedOn.Month}{user.CreatedOn.Day}{user.CreatedOn.Hour}{user.CreatedOn.Minute}";
                user.Email = $"{prefix}_{user.Email}";
                user.NormalizedEmail = user.Email.ToUpper();
                user.UserName = $"{prefix}_{user.UserName}";
                user.NormalizedUserName = user.UserName.ToUpper();
                user.PhoneNumber = $"{prefix}_{user.PhoneNumber}";
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
