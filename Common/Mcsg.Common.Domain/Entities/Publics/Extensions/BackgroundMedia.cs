namespace Mcsg.Common.Domain.Entities;

using Core.Extensions;
using SeedWork.Extensions;

partial class BackgroundMedia
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public BackgroundMedia()
    {
        CreatedOn = DateTime.UtcNow;
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
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto(string mediaApiUrl)
    {
        return ToBaseDto<SearchDto>(mediaApiUrl);
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto(string mediaApiUrl)
    {
        return ToBaseDto<ViewDto>(mediaApiUrl);
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>(string mediaApiUrl) where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
            Title = Title,
            Url = mediaApiUrl.GetMediaPath(".mp3", Url, null),
            Thumbnail = mediaApiUrl.GetMediaPath(".jpg", Thumbnail, null),
            ArtistName = ArtistName,
            Duration = DurationSeconds.ToDuration(),
            Order = Order
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto
    {
        #region -- Properties --

        /// <summary>
        /// Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// Thumbnail
        /// </summary>
        public string? Thumbnail { get; set; }

        /// <summary>
        /// ArtistName
        /// </summary>
        public string? ArtistName { get; set; }

        /// <summary>
        /// Duration
        /// </summary>
        public string? Duration { get; set; }

        /// <summary>
        /// Order
        /// </summary>
        public int Order { get; set; }

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
    }

    #endregion
}
