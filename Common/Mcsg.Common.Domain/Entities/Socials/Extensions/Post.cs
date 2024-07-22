namespace Mcsg.Common.Domain.Entities;

using Core.Constants;
using Core.Enums;
using SeedWork.Extensions;

public partial class Post
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Post()
    {
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="body">Body</param>
    /// <param name="thumbnailUrl">Thumbnail URL</param>
    /// <param name="authorName">Author name</param>
    /// <param name="customNote">Custom note</param>
    /// <param name="createdBy">Created by</param>
    /// <returns>Return the result</returns>
    public static Post Create(string? title, string body, string? thumbnailUrl, string? authorName, string? customNote, Guid createdBy)
    {
        var hashId = Setting.PostConfig.HashLength.GetRandomString();

        var res = new Post
        {
            Title = title,
            Type = (int)PostType.Feed,
            Status = PostStatus.Public,
            HashId = hashId,
            Body = body,
            ThumbnailUrl = thumbnailUrl,
            AuthorName = authorName,
            CustomNote = customNote,
            UserId = createdBy,
            CreatedBy = createdBy
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="title">Title</param>
    /// <param name="body">Body</param>
    /// <param name="thumbnailUrl">Thumbnail URL</param>
    /// <param name="customNote">Custom note</param>
    /// <param name="modifiedBy">Modified by</param>
    public void Update(string? title, string body, string? thumbnailUrl, string? customNote, Guid modifiedBy)
    {
        Title = title;
        Body = body;
        ThumbnailUrl = thumbnailUrl;
        CustomNote = customNote;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="modifiedBy">Modified by</param>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;

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
        return ToBaseDto<ViewDto>();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            //TODO
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto
    {
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
    }

    #endregion
}
