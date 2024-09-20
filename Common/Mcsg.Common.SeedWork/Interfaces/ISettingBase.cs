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

namespace Mcsg.Common.SeedWork.Interfaces;

using Dtos;
using Enums;
using static Dtos.ConnectionDto;
using static Dtos.StorageDto;

/// <summary>
/// Interface setting base
/// </summary>
public interface ISettingBase
{
    #region -- Methods --

    /// <summary>
    /// Retrieves a MinIO instance based on the specified instance type.
    /// </summary>
    /// <param name="instance">The type of the MinIO instance to retrieve.</param>
    /// <returns>The corresponding <see cref="MinioInstanceDto"/> object.</returns>
    MinioInstanceDto GetMinio(MinioInstanceType instance);

    #endregion

    #region -- Properties --

    /// <summary>
    /// Project prefix
    /// </summary>
    string Prefix { get; set; }

    /// <summary>
    /// Environment (local, dev, uat, stg and pro)
    /// </summary>
    string Environment { get; set; }

    /// <summary>
    /// Site name
    /// </summary>
    string SiteName { get; set; }

    /// <summary>
    /// For detecting mobile to call API
    /// </summary>
    string MobileUserAgent { get; set; }

    /// <summary>
    /// Domain
    /// </summary>
    string Domain { get; set; }

    /// <summary>
    /// Swagger enabled
    /// </summary>
    bool SwaggerEnabled { get; set; }

    /// <summary>
    /// Development mode
    /// </summary>
    bool DevMode { get; set; }

    /// <summary>
    /// Protocols (An example such as 'Http1_80;Http2_81' means that Http1 runs on port 80 and Http2 runs on port 81)
    /// </summary>
    string? Protocols { get; set; }

    /// <summary>
    /// Information
    /// </summary>
    public string? Information { get; }

    /// <summary>
    /// Is production mode
    /// </summary>
    bool IsProduction { get; }

    /// <summary>
    /// Is local mode
    /// </summary>
    bool IsLocal { get; }

    /// <summary>
    /// JSON Web Token
    /// </summary>
    JwtDto Jwt { get; }

    /// <summary>
    /// Database
    /// </summary>
    DatabaseDto Db { get; }

    /// <summary>
    /// Email
    /// </summary>
    NotificationDto Email { get; }

    /// <summary>
    /// Queue
    /// </summary>
    QueueDto Queue { get; }

    /// <summary>
    /// MinIO
    /// </summary>
    MinioDto Minio { get; }

    /// <summary>
    /// API
    /// </summary>
    ApiDto Api { get; }

    /// <summary>
    /// RPC
    /// </summary>
    ApiDto Rpc { get; }

    /// <summary>
    /// The origins that are allowed (CORS)
    /// </summary>
    string? Origins { get; set; }

    /// <summary>
    /// Encrypt key
    /// </summary>
    string EncryptKey { get; set; }

    #endregion
}
