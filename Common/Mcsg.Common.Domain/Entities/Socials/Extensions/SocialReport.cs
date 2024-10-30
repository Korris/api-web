using System.Text.Json.Serialization;

namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Dtos;

partial class SocialReport
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SocialReport()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="entityId"></param>
    /// <param name="entityType"></param>
    /// <param name="status"></param>
    /// <param name="ExpiredBlock"></param>
    /// <returns>Return the result</returns>
    public static SocialReport Create(Guid entityId, EntityType entityType, ReportStatus status, DateTime? ExpiredBlock)
    {
        var res = new SocialReport
        {
            EntityId = entityId,
            EntityType = entityType,
            Status = status,
            ExpiredBlock = ExpiredBlock
        };

        return res;
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
        return ToBaseDto<ViewDto>();
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>() where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
            EntityId = EntityId,
            EntityType = EntityType.ToString(),
            Status = Status,
            ExpiredBlock = ExpiredBlock
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : IdDto
    {
        #region -- Properties --

        /// <summary>
        /// EntityId
        /// </summary>
        public Guid? EntityId { get; set; }

        /// <summary>
        /// Entity type
        /// </summary>
        public string? EntityType { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        [JsonIgnore]
        public ReportStatus Status { get; set; }

        /// <summary>
        /// StatusName
        /// </summary>
        public string StatusName => Status.ToString();

        /// <summary>
        /// Expired block
        /// </summary>
        public DateTime? ExpiredBlock { get; set; }

        #endregion
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
    }

    #endregion
}
