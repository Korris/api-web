using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Interfaces;

/// <summary>
/// DeleteAccount service
/// </summary>
public class DeleteAccountService : IDeleteAccountService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public DeleteAccountService(IMcsgContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <returns>Return the result</returns>
    public async Task Run()
    {
        try
        {
            var take = 5;
            var q = _context.Users.Where(p => p.IsDelete);

            // Change status from WillDelete to Deleted
            var utc = DateTime.UtcNow.AddMinutes(-_setting.AccountDeletedAfter);
            var willDeleteUsers = await q.Where(p => p.Status != UserStatus.WillDelete && p.Status != UserStatus.Deleted && p.DeletedAt < utc)
                .Take(take).ToListAsync();
            foreach (var i in willDeleteUsers)
            {
                i.Status = UserStatus.WillDelete;
            }

            // Add a prefix to email and phone numbers to allow users to create new accounts
            utc = DateTime.UtcNow.AddMinutes(-_setting.AccountCreatedAfter);
            var deletedUsers = await q.Where(p => p.Status == UserStatus.WillDelete && p.DeletedAt < utc)
                .Take(take).ToListAsync();
            foreach (var i in deletedUsers)
            {
                var prefix = $"d_{i.CreatedOn.Month}{i.CreatedOn.Day}{i.CreatedOn.Hour}{i.CreatedOn.Minute}_";

                var email = i.Email + "";
                var phone = i.PhoneNumber + "";

                email = email.Replace(prefix, "");
                phone = phone.Replace(prefix, "");

                i.Email = $"{prefix}{email}";
                i.PhoneNumber = $"{prefix}{phone}";

                i.NormalizedEmail = i.Email.ToUpper();

                i.Status = UserStatus.Deleted;
            }

            if (willDeleteUsers.Count > 0 || deletedUsers.Count > 0)
            {
                await _context.SaveChangesAsync(default);
            }

            var userIds = deletedUsers.Select(p => p.Id).ToList();
            if (userIds.Count > 0)
            {
                await _context.Available<UserSocial>()
                    .Where(p => userIds.Contains(p.UserId))
                    .ExecuteUpdateAsync(p => p.SetProperty(q => q.IsDelete, true), default);
            }
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
