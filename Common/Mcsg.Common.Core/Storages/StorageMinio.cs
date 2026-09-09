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
using SixLabors.ImageSharp;
using System.Net;

namespace Mcsg.Common.Core.Storages;

using Dtos;
using Enums;
using Minio.Exceptions;
using static Common.Core.Constants.Setting;

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
    public override async Task PutObject(Stream fs, string objectName, string? bucketName, string contentType)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        fs.Position = 0;
        var putArg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(fs).WithObjectSize(fs.Length).WithContentType(contentType);
        await Mc.PutObjectAsync(putArg);
    }

    public override async Task PutObject(Stream fs, string objectName, string? bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        // Ensure the position is at the beginning of the Stream
        fs.Position = 0;

        var putArg = new PutObjectArgs().WithBucket(bucketName).WithObject(objectName).WithStreamData(fs).WithObjectSize(fs.Length);

        var ext = Path.GetExtension(objectName);
        if (FileExt.Documents.Contains(ext))
        {
            _dicMimeType.TryGetValue(ext, out var mimeType);
            putArg = putArg.WithContentType(mimeType);
        }

        await Mc.PutObjectAsync(putArg);
    }

    /// <summary>
    /// Uploads an object to a bucket.
    /// </summary>
    /// <param name="url">The URL of the file to upload.</param>
    /// <param name="objectName">The name of the object (including full path and file extension).</param>
    /// <param name="bucketName">The name of the bucket. If null, the default bucket from the settings will be used.</param>
    /// <param name="isOverwrite">Indicates whether to overwrite the object if it already exists.</param>
    /// <param name="timeout">The timeout for the HTTP request.</param>
    /// <returns>A task representing the asynchronous operation, with an ImageRatio object if successful, or null if not.</returns>
    public override async Task<ImageRatio?> PutObject(string url, string objectName, string? bucketName, bool isOverwrite, TimeSpan timeout)
    {
        if (!isOverwrite)
        {
            var stat = await StatObject(objectName, bucketName);
            if (stat != null)
            {
                return new ImageRatio(); // object already exists, and overwrite is not allowed.
            }
        }

        using HttpClient client = new() { Timeout = timeout };
        HttpResponseMessage response;
        try
        {
            response = await client.GetAsync(url);
        }
        catch (Exception)
        {
            return null;
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            return null;
        }

        try
        {
            MemoryStream ms = new();
            await response.Content.CopyToAsync(ms);
            ms.Position = 0;

            using var image = await Image.LoadAsync(ms);
            ms.Position = 0;

            await PutObject(ms, objectName, bucketName);

            return new ImageRatio
            {
                Width = image.Width,
                Height = image.Height,
                Stream = ms,
                ObjectName = objectName
            };
        }
        catch (Exception)
        {
            return null;
        }
    }

    /// <summary>
    /// Presigned get object
    /// </summary>
    /// <param name="objectName">Object name</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <param name="expiry">Expiry in seconds</param>
    /// <returns>Return the result</returns>
    public override async Task<string> PresignedGetObject(string objectName, string? bucketName, int? expiry = null)
    {
        if (string.IsNullOrWhiteSpace(objectName))
        {
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        if (expiry == null || expiry <= 0)
        {
            expiry = _maxExpiryInSeconds;
        }

        try
        {
            var presignedArg = new PresignedGetObjectArgs().WithBucket(bucketName).WithObject(objectName).WithExpiry(expiry.Value);
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
    public override async Task<ObjectStat?> StatObject(string objectName, string? bucketName)
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
        catch (ObjectNotFoundException)
        {
            // Trường hợp bình thường: file không tồn tại
            return null;
        }
        catch (MinioException me)
        {
            // Bucket/region/endpoint sai: caller không phân biệt được nên ghi rõ loại lỗi và bucket
            Console.WriteLine($"[StatObject] {me.GetType().Name} bucket={bucketName} object={objectName}: {me.Message}");
            return null;
        }
        catch (NullReferenceException)
        {
            // Minio SDK 6.0.5: HEAD bị MinIO trả mã lỗi khác 404 (403/400/405/501) làm vỡ bộ parse lỗi của SDK
            // (ParseWellKnownErrorNoContent gọi response.Exception.ToString() khi Exception null), mã lỗi thật bị mất.
            // Dùng list theo prefix để vẫn nhận ra file đang tồn tại, tránh việc bỏ qua bước move ở caller.
            Console.WriteLine($"[StatObject] HEAD rejected with a non-404 status (hidden by Minio SDK 6.0.5) bucket={bucketName} object={objectName}; falling back to list");
            return await StatByListing(objectName, bucketName);
        }
        catch (Exception ex)
        {
            // Lỗi ngoài MinIO (DNS, TLS, timeout) trước đây bị nuốt hoàn toàn
            Console.WriteLine($"[StatObject] {ex.GetType().Name} bucket={bucketName} object={objectName}: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Existence check that does not depend on HEAD: lists the exact key and rebuilds an ObjectStat from the listing entry
    /// </summary>
    private async Task<ObjectStat?> StatByListing(string objectName, string? bucketName)
    {
        try
        {
            var listArg = new ListObjectsArgs().WithBucket(bucketName).WithPrefix(objectName).WithRecursive(true);
            await foreach (var item in Mc.ListObjectsEnumAsync(listArg))
            {
                if (item.IsDir || !string.Equals(item.Key, objectName, StringComparison.Ordinal))
                {
                    continue;
                }

                var headers = new Dictionary<string, string>
                {
                    ["Content-Length"] = item.Size.ToString(),
                    ["ETag"] = item.ETag ?? string.Empty
                };
                if (!string.IsNullOrWhiteSpace(item.LastModified))
                {
                    headers["Last-Modified"] = item.LastModified;
                }

                return ObjectStat.FromResponseHeaders(objectName, headers);
            }

            Console.WriteLine($"[StatObject] list fallback: object not found bucket={bucketName} object={objectName}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[StatObject] list fallback failed {ex.GetType().Name} bucket={bucketName} object={objectName}: {ex.Message}");
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
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <returns>Return the public URL</returns>
    public override async Task<string> GetPublicUrl(string objectName, string? bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            bucketName = _auth?.BucketName;
        }

        var uri = await PresignedGetObject(objectName, bucketName);
        if (bucketName != BucketNamePublic)
        {
            return uri;
        }

        var arr = uri.Split('?');
        return arr.Length > 0 ? arr[0] : "";
    }

    /// <summary>
    /// Get CDN URL
    /// </summary>
    /// <param name="objectName">Object name (include full path and file extension)</param>
    /// <param name="bucketName">Bucket name (if it is null, get the default from the setting)</param>
    /// <param name="type">Resource type</param>
    /// <returns>Return the CDN URL</returns>
    public override async Task<string> GetCdnUrlAsync(string objectName, string? bucketName, ResourceType? type)
    {
        if (_auth != null && _auth.UseCdn != true)
        {
            return _auth.GetPublicUrl(bucketName, objectName);
        }

        var uri = await PresignedGetObject(objectName, bucketName);

        var url = $"{_auth?.PublicUrl}/{bucketName}";
        if (type == ResourceType.Image && !string.IsNullOrWhiteSpace(_auth?.CdnImageUrl))
        {
            uri = uri.Replace(url, _auth?.CdnImageUrl);
        }
        else if (type == ResourceType.Video && !string.IsNullOrWhiteSpace(_auth?.CdnVideoUrl))
        {
            uri = uri.Replace(url, _auth?.CdnVideoUrl);
        }

        var hasCdn = !uri.Contains(url);
        if (hasCdn)
        {
            var arr = uri.Split('?');
            return arr.Length > 0 ? arr[0] : "";
        }

        return uri;
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
    /// <returns>Return the result</returns>
    private IMinioClient Mc
    {
        get
        {
            if (_mc != null)
            {
                return _mc;
            }

            ArgumentNullException.ThrowIfNull(_auth, nameof(_auth));

            _mc = new MinioClient().WithEndpoint(_auth.EndPoint).WithCredentials(_auth.AccessKey, _auth.SecretKey).WithRegion(_auth.Location);
            if (_auth.PublicUrl.Contains("https"))
            {
                _mc = _mc
                    .WithSSL(true)
                    .WithHttpClient(new HttpClient(new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
                    }));
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

    /// <summary>
    /// The maximum expiry time in seconds (default is 1 days).
    /// </summary>
    private int _maxExpiryInSeconds = 1 * 24 * 60 * 60; // 1 days

    /// <summary>
    /// Dictionary mime type
    /// </summary>
    private Dictionary<string, string> _dicMimeType = new()
    {
        { ".pdf", "application/pdf" },
        { ".ppt", "application/vnd.ms-powerpoint" },
        { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
        { ".doc", "application/msword" },
        { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    #endregion
}
