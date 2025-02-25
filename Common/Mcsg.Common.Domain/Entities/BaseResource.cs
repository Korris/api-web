using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork;
using SeedWork.Constants;
using SeedWork.Dtos;
using SeedWork.Enums;

public class BaseResource : AuditableEntity
{
    #region -- Methods --

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="file"></param>
    /// <param name="hashId"></param>
    /// <param name="name"></param>
    /// <param name="url"></param>
    /// <param name="bucketName"></param>
    /// <param name="type"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="compressedSize"></param>
    /// <param name="minioInstance"></param>
    /// <param name="createdBy"></param>
    /// <returns></returns>
    public static SocialResource Create(IFormFile file, string hashId, string name, string url, string bucketName, ResourceType type, int width, int height, double compressedSize, MinioInstanceType? minioInstance, Guid createdBy)
    {
        var res = new SocialResource
        {
            AuthorId = createdBy,
            HashId = hashId,
            Title = Path.GetFileNameWithoutExtension(file.FileName),
            Name = name,
            Url = url,
            BucketName = bucketName,
            Type = type,
            Width = width,
            Height = height,
            Size = file.Length,
            CompressedSize = compressedSize,
            MinioInstance = minioInstance,
            IsDelete = true,
            CreatedBy = createdBy
        };

        return res;
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
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto()
    {
        return ToBaseDto<SearchDto>();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto()
    {
        var res = ToBaseDto<ViewDto>();

        res.HashId = HashId;
        res.ObjectName = Url;
        res.BucketName = BucketName;
        res.MinioInstance = MinioInstance;
        res.MicroService = MicroService;

        return res;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            Id = Id
        };
    }

    #endregion

    #region -- Properties --

    [StringLength(Validator.Title.Max)]
    public string? Title { get; set; }

    [StringLength(Validator.Name.Max)]
    public string? Name { get; set; }

    [StringLength(Validator.Hashtag.Max)]
    public string? HashId { get; set; }

    /// <summary>
    /// This object name
    /// </summary>
    [StringLength(Validator.Url.Max)]
    public string? Url { get; set; }

    /// <summary>
    /// Bucket name
    /// </summary>
    [StringLength(32)]
    public string? BucketName { get; set; }

    /// <summary>
    /// Minio instance
    /// </summary>
    public MinioInstanceType? MinioInstance { get; set; }

    public Guid? AuthorId { get; set; }
    public Guid? SubPostId { get; set; }
    public int Order { get; set; }

    /// <summary>
    /// Size (byte)
    /// </summary>
    public double Size { get; set; }

    /// <summary>
    /// Size of the compressed file in bytes.
    /// </summary>
    public double CompressedSize { get; set; }

    public int Width { get; set; }
    public int Height { get; set; }
    public ResourceType Type { get; set; }
    public ResourceLocationType LocationType { get; set; }
    public ResourceStatus Status { get; set; } = ResourceStatus.Done;

    [StringLength(Validator.Url.Max)]
    public string? ExternalUrl { get; set; }
    public ExternalResource? ExternalResource { get; set; }

    /// <summary>
    /// MicroService
    /// </summary>
    [NotMapped]
    public string MicroService { get; set; } = Core.Enums.MicroService.Social.ToString();

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : IdDto
    {
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
        #region -- Properties --

        /// <summary>
        /// Hash ID
        /// </summary>
        public string? HashId { get; set; }

        /// <summary>
        /// This object name
        /// </summary>
        public string? ObjectName { get; set; }

        /// <summary>
        /// Bucket name
        /// </summary>
        public string? BucketName { get; set; }

        /// <summary>
        /// Minio instance
        /// </summary>
        public MinioInstanceType? MinioInstance { get; set; }

        /// <summary>
        /// Micro service
        /// </summary>
        public string? MicroService { get; set; }

        #endregion
    }

    #endregion
}
