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
/// Minio data transfer object
/// </summary>
public class MinioDto
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public MinioDto() { }

    #endregion

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
    /// EndPoint
    /// </summary>
    public string EndPoint { get; set; } = default!;

    /// <summary>
    /// Public URL
    /// </summary>
    public string PublicUrl { get; set; } = default!;

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