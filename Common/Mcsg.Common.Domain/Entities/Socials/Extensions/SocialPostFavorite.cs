namespace Mcsg.Common.Domain.Entities;

using SeedWork.Dtos;

partial class SocialPostFavorite
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SocialPostFavorite()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <param name="createdBy">CreatedBy</param>
    /// <returns>Return the result</returns>
    public static SocialPostFavorite Create(Guid postId, Guid createdBy)
    {
        var res = new SocialPostFavorite
        {
            PostId = postId,
            UserId = createdBy,
            CreatedBy = createdBy,
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Update(Guid modifiedBy)
    {
        IsDelete = !IsDelete;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto()
    {
        return ToBaseDto<SearchDto>();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto()
    {
        var res = ToBaseDto<ViewDto>();

        res.IsFavorite = !IsDelete;

        return res;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
            PostId = PostId,
            UserId = UserId,
            IsDelete = IsDelete,
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : IdDto
    {
        #region -- Properties --

        public Guid PostId { get; set; }

        public Guid UserId { get; set; }

        public bool IsDelete { get; set; }

        #endregion
    }

    /// <summary>
    /// Search
    /// </summary>
    public class SearchDto : BaseDto
    {
    }

    /// <summary>
    /// View
    /// </summary>
    public class ViewDto : BaseDto
    {
        #region -- Properties --

        public bool IsFavorite { get; set; }

        #endregion
    }

    #endregion
}
