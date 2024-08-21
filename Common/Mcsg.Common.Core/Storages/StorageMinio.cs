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
    public override async Task<Stream?> GetObject(string objectName, string? bucketName)
    {
        Stream? res = null;

        if (string.IsNullOrWhiteSpace(objectName))
        {
            return null;
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        try
        {
            res = new MemoryStream();
            var getArg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithCallbackStream(p => { p.CopyTo(res); });
            await Mc.GetObjectAsync(getArg);
        }
        catch
        {
            res = null;
        }

        return res;
    }

    /// <summary>
    /// Download object
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="fileName">File name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task DownloadObject(string objectName, string fileName, string? bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        var getArg = new GetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithFile(fileName);
        await Mc.GetObjectAsync(getArg);
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
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        // Ensure the position is at the beginning of the Stream
        fs.Position = 0;

        var putArg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(fs).WithObjectSize(fs.Length);
        await Mc.PutObjectAsync(putArg);
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
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        try
        {
            var presignedArg = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiry);
            return await Mc.PresignedGetObjectAsync(presignedArg);
        }
        catch
        {
            return Mc.Config.Endpoint;
        }
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
        if (string.IsNullOrWhiteSpace(srcBucketName))
        {
            srcBucketName = _auth?.BucketName;
        }
        if (string.IsNullOrWhiteSpace(dstBucketName))
        {
            dstBucketName = _auth?.BucketName;
        }

        var copySrcArg = new CopySourceObjectArgs().WithBucket(srcBucketName).WithObject(srcObjectName);
        var copyArg = new CopyObjectArgs().WithBucket(dstBucketName).WithObject(dstObjectName).WithCopyObjectSource(copySrcArg);
        await Mc.CopyObjectAsync(copyArg);

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
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        try
        {
            var statArg = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
            return await Mc.StatObjectAsync(statArg);
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
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        var statArg = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
        await Mc.RemoveObjectAsync(statArg);

        return true;
    }

    /// <summary>
    /// Get public URL
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketNamePublic">Bucket name public (if it is null, get the default from the setting)</param>
    /// <returns>Return the public URL</returns>
    public override async Task<string> GetPublicUrl(string objectName, string? bucketNamePublic)
    {
        if (string.IsNullOrWhiteSpace(bucketNamePublic))
        {
            bucketNamePublic = BucketNamePublic;
        }

        var uri = await PresignedGetObject(objectName, 1, bucketNamePublic);
        var arr = uri.Split('?');
        return arr.Length > 0 ? arr[0] : "";
    }

    /// <summary>
    /// Move folder
    /// </summary>
    /// <param name="srcFolder">Source folder</param>
    /// <param name="dstFolder">Destination folder</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the result</returns>
    public override async Task<int> MoveFolder(string srcFolder, string dstFolder, string? bucketName)
    {
        var res = 0;

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        // List objects in the source folder
        var objectsList = new List<string>();
        var getObjectsArgs = new ListObjectsArgs().WithBucket(bucketName).WithPrefix(srcFolder).WithRecursive(true);
        await foreach (var i in Mc.ListObjectsEnumAsync(getObjectsArgs))
        {
            objectsList.Add(i.Key);
        }

        // Copy each object to the destination folder and delete the original
        foreach (string obj in objectsList)
        {
            var destObj = obj.Replace(srcFolder, dstFolder);

            var copySourceArgs = new CopySourceObjectArgs().WithBucket(bucketName).WithObject(obj);
            var copyObjectArgs = new CopyObjectArgs().WithBucket(bucketName).WithObject(destObj).WithCopyObjectSource(copySourceArgs);
            await Mc.CopyObjectAsync(copyObjectArgs);

            var removeObjectArgs = new RemoveObjectArgs().WithBucket(bucketName).WithObject(obj);
            await Mc.RemoveObjectAsync(removeObjectArgs);

            res++;
        }

        return res;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// MinIO client
    /// </summary>
    /// <returns></returns>
    private IMinioClient Mc
    {
        get
        {
            if (_mc != null)
            {
                return _mc;
            }

            ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

            _mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecrectKey).WithRegion(_auth.Location);
            if (_auth.PublicUrl.Contains("https"))
            {
                _mc.WithSSL();
            }

            _mc = _mc.Build();

            return _mc;
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// MinIO client
    /// </summary>
    private IMinioClient? _mc;

    #endregion
}
