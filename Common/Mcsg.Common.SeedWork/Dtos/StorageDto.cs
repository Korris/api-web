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
    /// With SSL
    /// </summary>
    public bool WithSSL { get; set; } = true;

    /// <summary>
    /// Public URL
    /// </summary>
    public string PublicUrl { get; set; } = default!;

    /// <summary>
    /// Media encrypt key
    /// </summary>
    public string MediaEncryptKey { get; set; } = default!;

    /// <summary>
    /// Media API URL
    /// </summary>
    public string MediaApiUrl { get; set; } = default!;

    /// <summary>
    /// Media CDN URL
    /// </summary>
    public string MediaCdnUrl { get; set; } = default!;

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

        #endregion
    }

    #endregion
}
