using System.Text.Json.Serialization;

namespace Mcsg.Common.Domain.Entities;

using Core.Constants;
using Core.Enums;
using SeedWork.Converters;
using SeedWork.Dtos;
using SeedWork.Extensions;

partial class ComicPost
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicPost()
    {
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
    public static ComicPost Create(string? title, string? body, string? thumbnailUrl, string? authorName, string? customNote, Guid createdBy)
    {
        var hashId = Setting.PostConfig.HashLength.GetRandomString();

        var res = new ComicPost
        {
            Title = title,
            Type = PostType.Comic,
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
    public void Update(string? title, string? body, string? thumbnailUrl, string? customNote, Guid modifiedBy)
    {
        Title = title;
        Body = body;
        ThumbnailUrl = thumbnailUrl;
        CustomNote = customNote;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="hide">Hide option</param>
    /// <param name="modifiedBy">Modified by</param>
    public void Update(HideOption hide, Guid modifiedBy)
    {
        Hide = hide;

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
        var res = ToBaseDto<SearchDto>();

        res.Permission = Permission;
        res.NumberOfComments = ComicPostComments.Count;
        res.NumberOfFavorites = ComicPostFavorites.Count;
        res.NumberOfSubPosts = ComicSubPosts.Count;
        res.NumberOfViews = NumberOfViews;
        res.Tags = ComicTagPosts.Select(t => t.Tag.Name).ToList();

        return res;
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
            Id = Id,
            HashId = HashId,
            Title = Title,
            Body = Body,
            CreatedOn = CreatedOn,
            ThumbnailUrl = ThumbnailUrl,
            ProfileName = User.ProfileName,
            UserName = User.UserName,
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

        /// <summary>
        /// HashId
        /// </summary>
        public string? HashId { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public string? Body { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? CreatedOn { get; set; }


        /// <summary>
        /// ThumbnailUrl
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// ProfileName
        /// </summary>
        public string? ProfileName { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string? UserName { get; set; }

        #endregion
    }

    /// <summary>
    /// Search
    /// </summary>
    public class SearchDto : BaseDto
    {
        #region -- Properties --

        /// <summary>
        /// Permission
        /// </summary>
        [JsonIgnore]
        public PostPermission Permission { get; set; }

        /// <summary>
        /// PermissionName
        /// </summary>
        public string PermissionName => Permission.ToString();

        /// <summary>
        /// NumberOfComments
        /// </summary>
        public int NumberOfComments { get; set; }

        /// <summary>
        /// NumberOfFavorites
        /// </summary>
        public int NumberOfFavorites { get; set; }

        /// <summary>
        /// NumberOfSubPosts
        /// </summary>
        public int NumberOfSubPosts { get; set; }

        /// <summary>
        /// NumberOfViews
        /// </summary>
        public int NumberOfViews { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public List<string?> Tags { get; set; } = [];

        #endregion
    }

    /// <summary>
    /// View
    /// </summary>
    public class ViewDto : BaseDto
    {
    }

    #endregion
}
