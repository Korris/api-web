using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Extensions;
using Common.Domain;
using Interfaces;
using static Common.SeedWork.Constants.Setting;

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

            var connection = _context.Database.GetDbConnection();
            await connection.OpenAsync();

            var userIds = users.Select(p => p.Id).ToList();
            foreach (var userId in userIds)
            {
                var postIds = await _context.ComicPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
                var now = DateTime.UtcNow;
                var sql = "CALL comic.sp_delete_post_and_related_data(@PostId, @ModifiedBy, @ModifiedOn);";
                foreach (var postId in postIds)
                {
                    var param = new { PostId = postId, ModifiedBy = CreatedBy.System, ModifiedOn = now };
                    var data = await connection.QueryAsync(sql, param);
                }

                postIds = await _context.SocialPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
                now = DateTime.UtcNow;
                sql = "CALL social.sp_delete_post_and_related_data(@PostId, @ModifiedBy, @ModifiedOn);";
                foreach (var postId in postIds)
                {
                    var param = new { PostId = postId, ModifiedBy = CreatedBy.System, ModifiedOn = now };
                    var data = await connection.QueryAsync(sql, param);
                }

                postIds = await _context.StoryPostAvailable.Where(p => p.UserId == userId).Select(p => p.Id).ToListAsync();
                now = DateTime.UtcNow;
                sql = "CALL story.sp_delete_post_and_related_data(@PostId, @ModifiedBy, @ModifiedOn);";
                foreach (var postId in postIds)
                {
                    var param = new { PostId = postId, ModifiedBy = CreatedBy.System, ModifiedOn = now };
                    var data = await connection.QueryAsync(sql, param);
                }
            }

            users.ForEach(p => p.IsDelete = true);
            await _context.SaveChangesAsync(default);

            await connection.CloseAsync();
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
