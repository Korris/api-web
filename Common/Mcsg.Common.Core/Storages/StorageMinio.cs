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

namespace Mcsg.Common.Core.Storages;

using Minio;
using Minio.DataModel.Args;

/// <summary>
/// Storage MinIO
/// </summary>
public class StorageMinio : StorageStrategy
{
    #region -- Overrides --

    /// <summary>
    /// Get object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<Stream> GetObject(string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        var res = new MemoryStream();

        if (string.IsNullOrWhiteSpace(objectName))
        {
            return res;
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var statArg = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
        await mc.StatObjectAsync(statArg);

        var getArg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithCallbackStream(p => { p.CopyTo(res); });
        await mc.GetObjectAsync(getArg);

        return res;
    }

    /// <summary>
    /// Put object
    /// </summary>
    /// <param name="fs">Stream</param>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task PutObject(Stream fs, string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var putArg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(fs).WithObjectSize(fs.Length);
        await mc.PutObjectAsync(putArg);
    }

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns></returns>
    public override async Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var putArg = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiry);
        return await mc.PresignedGetObjectAsync(putArg);
    }

    #endregion
}
