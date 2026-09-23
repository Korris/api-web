using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Services;

using Common.Domain;
using Common.Domain.Entities;
using Mcsg.Api.Interfaces;

/// <summary>
/// Hashtag links for Game / TapShow posts (see IPostHashtagService). One implementation for every
/// link table that derives from BaseTagPost; the table is picked by the generic parameter.
/// </summary>
public class PostHashtagService : IPostHashtagService
{
    #region -- Methods --

    public PostHashtagService(IMcsgContext context)
    {
        _context = context;
    }

    public List<string> Normalize(IEnumerable<string>? tags)
    {
        var result = new List<string>();
        if (tags == null)
        {
            return result;
        }
        foreach (var raw in tags)
        {
            var name = (raw ?? string.Empty).Trim().TrimStart('#').Trim().ToLowerInvariant();
            if (name.Length > 0 && !result.Contains(name))
            {
                result.Add(name);
            }
        }
        return result;
    }

    public async Task SetAsync<TTagPost>(Guid postId, IEnumerable<string>? tags, Guid userId) where TTagPost : BaseTagPost, new()
    {
        var names = Normalize(tags);
        var links = await _context.Set<TTagPost>().Where(l => l.PostId == postId).ToListAsync();

        // Existing Tag rows by name (the Tags table is not unique on Name: keep the oldest per name)
        var tagIdByName = new Dictionary<string, Guid>(StringComparer.Ordinal);
        if (names.Count > 0)
        {
            var found = await _context.Available<Tag>(false)
                .Where(t => t.Name != null && names.Contains(t.Name))
                .OrderBy(t => t.CreatedOn)
                .Select(t => new { t.Id, t.Name })
                .ToListAsync();
            foreach (var t in found)
            {
                tagIdByName.TryAdd(t.Name!, t.Id);
            }
        }

        // Tags nobody used before → new rows in public."Tags"
        foreach (var name in names.Where(n => !tagIdByName.ContainsKey(n)))
        {
            var tag = new Tag { Name = name, Title = name, AuthorId = userId, CreatedBy = userId, ModifiedBy = userId };
            await _context.Tags.AddAsync(tag);
            tagIdByName[name] = tag.Id;
        }

        var wanted = names.Select(n => tagIdByName[n]).ToHashSet();
        var now = DateTime.UtcNow;

        // Existing links: revive the wanted ones, soft-delete the rest
        foreach (var link in links)
        {
            var keep = wanted.Contains(link.TagId);
            if (link.IsDelete == !keep)
            {
                continue;
            }
            link.IsDelete = !keep;
            link.ModifiedBy = userId;
            link.ModifiedOn = now;
        }

        // Wanted tags without a link yet
        var linked = links.Select(l => l.TagId).ToHashSet();
        foreach (var tagId in wanted.Where(id => !linked.Contains(id)))
        {
            await _context.Set<TTagPost>().AddAsync(new TTagPost { PostId = postId, TagId = tagId, CreatedBy = userId, ModifiedBy = userId });
        }
    }

    #endregion

    #region -- Fields --

    private readonly IMcsgContext _context;

    #endregion
}
