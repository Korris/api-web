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

using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;

namespace Mcsg.Common.Core.Storages;

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

        // Ensure the position is at the beginning of the Stream
        fs.Position = 0;

        var putArg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(fs).WithObjectSize(fs.Length);
        await mc.PutObjectAsync(putArg);
    }

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<string> PresignedGetObject(string objectName, int expiry, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var presignedArg = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiry);
        return await mc.PresignedGetObjectAsync(presignedArg);
    }

    /// <summary>
    /// Copy a source object into a new destination object
    /// </summary>
    /// <param name="srcObjectName">Source object name</param>
    /// <param name="dstObjectName">Destination object name</param>
    /// <param name="srcBucketName">Source bucket name (if it is null, get the default from the setting)</param>
    /// <param name="dstBucketName">Destination bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<bool> CopyObject(string srcObjectName, string dstObjectName, string? srcBucketName, string? dstBucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(srcBucketName))
        {
            srcBucketName = _auth.BucketName;
        }
        if (string.IsNullOrWhiteSpace(dstBucketName))
        {
            dstBucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var copySrcArg = new CopySourceObjectArgs().WithBucket(srcBucketName).WithObject(srcObjectName);
        var copyArg = new CopyObjectArgs().WithBucket(dstBucketName).WithObject(dstObjectName).WithCopyObjectSource(copySrcArg);
        await mc.CopyObjectAsync(copyArg);

        return true;
    }

    /// <summary>
    /// Tests the object's existence and returns metadata about existing objects
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<ObjectStat?> StatObjectAsync(string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        try
        {
            var statArg = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
            return await mc.StatObjectAsync(statArg);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Removes an object with given name in specific bucket
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<bool> RemoveObject(string objectName, string? bucketName)
    {
        ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth.BucketName;
        }

        var mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location).Build();

        var statArg = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
        await mc.RemoveObjectAsync(statArg);

        return true;
    }

    #endregion
}
