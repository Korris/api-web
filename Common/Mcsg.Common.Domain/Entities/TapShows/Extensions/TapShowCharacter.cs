namespace Mcsg.Common.Domain.Entities;

/// <summary>
/// TapShowCharacter domain helpers (create / update / delete)
/// </summary>
partial class TapShowCharacter
{
    #region -- Methods --

    /// <summary>
    /// Constructor
    /// </summary>
    public TapShowCharacter()
    {
    }

    /// <summary>
    /// Create a character under a post
    /// </summary>
    public static TapShowCharacter Create(Guid postId, string name, string? avatarUrl, int order, Guid createdBy)
    {
        return new TapShowCharacter
        {
            PostId = postId,
            Name = name,
            AvatarUrl = avatarUrl,
            Order = order,
            CreatedBy = createdBy
        };
    }

    /// <summary>
    /// Update editable fields
    /// </summary>
    public void Update(string name, string? avatarUrl, int order, Guid modifiedBy)
    {
        Name = name;
        AvatarUrl = avatarUrl;
        Order = order;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft delete
    /// </summary>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    #endregion
}
