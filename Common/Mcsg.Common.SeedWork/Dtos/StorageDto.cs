#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Dtos;

/// <summary>
/// Storage data transfer object
/// </summary>
public abstract class StorageDto
{
    #region -- Classes --

    /// <summary>
    /// MinioInstance
    /// </summary>
    public class MinioInstanceDto
    {
        #region -- Methods --

        /// <summary>
        /// Get public URL
        /// </summary>
        /// <param name="bucketName">Bucket name</param>
        /// <param name="objectName">Object name</param>
        /// <returns>Returns the result</returns>
        public string GetPublicUrl(string? bucketName, string? objectName)
        {
            var path = $"/{PublicPrefix}/{bucketName}/{objectName}".Replace("//", "/");
            return $"{PublicUrl}{path}";
        }

        #endregion

        #region -- Properties --

        /// <summary>
        /// Minio instance
        /// </summary>
        public int Instance { get; set; }

        /// <summary>
        /// EndPoint
        /// </summary>
        public string EndPoint { get; set; } = default!;

        /// <summary>
        /// Public URL
        /// </summary>
        public string PublicUrl { get; set; } = default!;

        /// <summary>
        /// CDN image URL
        /// </summary>
        public string? CdnImageUrl { get; set; }

        /// <summary>
        /// CDN video URL
        /// </summary>
        public string? CdnVideoUrl { get; set; }

        /// <summary>
        /// Use CDN
        /// </summary>
        public bool UseCdn { get; set; }

        /// <summary>
        /// Public prefix
        /// </summary>
        public string? PublicPrefix { get; set; }

        /// <summary>
        /// Bucket name
        /// </summary>
        public string BucketName { get; set; } = default!;

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; } = default!;

        /// <summary>
        /// Access key
        /// </summary>
        public string AccessKey { get; set; } = default!;

        /// <summary>
        /// Secret key
        /// </summary>
        public string SecretKey { get; set; } = default!;

        /// <summary>
        /// Bucket name public
        /// </summary>
        public string BucketNamePublic => BucketName + "-public";

        #endregion
    }

    /// <summary>
    /// Represents the MinIO configuration.
    /// </summary>
    public class MinioDto
    {
        #region -- Properties --

        /// <summary>
        /// Gets or sets the list of MinIO instances.
        /// </summary>
        public List<MinioInstanceDto> Storages { get; set; } = [];

        /// <summary>
        /// Gets or sets the upload multipart body length limit in megabytes.
        /// </summary>
        public int UploadMultipartBodyLengthLimit { get; set; } = 128;

        /// <summary>
        /// Gets or sets the upload value length limit in megabytes.
        /// </summary>
        public int UploadValueLengthLimit { get; set; } = 4;

        /// <summary>
        /// Gets or sets the allowed media file extensions for comic.
        /// </summary>
        public string ComicMediaExtensionAllow { get; set; } = "jpg, jpeg, png, gif, bmp, heic, webp";

        /// <summary>
        /// Gets or sets the allowed media file extensions for document.
        /// </summary>
        public string DocumentMediaExtensionAllow { get; set; } = "jpg, jpeg, png, gif, bmp, heic, webp, ppt, pptx, doc, docx, pdf";

        /// <summary>
        /// Gets or sets the allowed media file extensions for social.
        /// </summary>
        public string SocialMediaExtensionAllow { get; set; } = "mp3, wav, ogg, jpg, jpeg, png, gif, bmp, mp4, avi, mkv, webm, heic, webp, mov";

        /// <summary>
        /// Gets or sets the allowed media file extensions for story.
        /// </summary>
        public string StoryMediaExtensionAllow { get; set; } = "jpg, jpeg, png, gif, bmp, heic, webp";

        /// <summary>
        /// Gets or sets the image downscaling quality percentage.
        /// </summary>
        public uint ImageDownQuality { get; set; } = 75;

        /// <summary>
        /// Gets or sets the maximum expiry time in seconds (default is 7 days).
        /// </summary>
        public int MaxExpiryInSeconds { get; set; } = 7 * 24 * 60 * 60; // 7 days

        #endregion
    }

    #endregion
}
