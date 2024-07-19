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
    #region -- Properties --

    /// <summary>
    /// EndPoint
    /// </summary>
    public string EndPoint { get; set; } = default!;

    /// <summary>
    /// Public URL
    /// </summary>
    public string PublicUrl { get; set; } = default!;

    /// <summary>
    /// Media API URL
    /// </summary>
    public string MediaApiUrl { get; set; } = default!;

    /// <summary>
    /// Upload multipart body length limit (MB)
    /// </summary>
    public int UploadMultipartBodyLengthLimit { get; set; } = 128;

    /// <summary>
    /// Upload value length limit (MB)
    /// </summary>
    public int UploadValueLengthLimit { get; set; } = 4;

    /// <summary>
    /// Media extension allow
    /// </summary>
    public string MediaExtensionAllow { get; set; } = "mp3, wav, ogg, jpg, jpeg, png, gif, bmp, mp4, webm, heic, webp, mov";

    /// <summary>
    /// Image down quality
    /// </summary>
    public int ImageDownQuality { get; set; } = 75;

    #endregion

    #region -- Classes --

    /// <summary>
    /// MinIO
    /// </summary>
    public class MinioDto : StorageDto
    {
        #region -- Properties --

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
        /// Secrect key
        /// </summary>
        public string SecrectKey { get; set; } = default!;

        /// <summary>
        /// Maximum expiry in seconds (7 days)
        /// </summary>
        public int MaxExpiryInSeconds { get; set; } = 7 * 24 * 60 * 60; // 7 days

        #endregion
    }

    #endregion
}
