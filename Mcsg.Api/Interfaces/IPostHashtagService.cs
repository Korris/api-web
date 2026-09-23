namespace Mcsg.Api.Interfaces;

using Common.Domain.Entities;

/// <summary>
/// Hashtags of a post for the areas that share one link-table shape (BaseTagPost: Game, TapShow).
/// Tags live in the shared public."Tags" table; each area keeps its own link table (game."GameTagPosts", ...).
/// Comic / Story / Document keep their older per-area TagService.
/// </summary>
public interface IPostHashtagService
{
    /// <summary>
    /// Normalize for storage: strip a leading '#', trim, lower-case, drop blanks and duplicates (order kept)
    /// </summary>
    List<string> Normalize(IEnumerable<string>? tags);

    /// <summary>
    /// Make the live links of the post match <paramref name="tags"/>: missing Tag rows are created,
    /// links for removed names are soft-deleted, links for re-added names are revived.
    /// null or empty → every link removed. Does NOT SaveChanges: the caller commits with the post.
    /// </summary>
    Task SetAsync<TTagPost>(Guid postId, IEnumerable<string>? tags, Guid userId) where TTagPost : BaseTagPost, new();
}
