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
    /// <param name="createdBy"></param>
    /// <returns>Return the result</returns>
    public static SocialReport Create(Guid entityId, EntityType entityType, Guid createdBy)
    {
        var res = new SocialReport
        {
            EntityId = entityId,
            EntityType = entityType,
            CreatedBy = createdBy
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
    /// <param name="reasonType"></param>
    /// <param name="reasonText"></param>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto(ReasonType reasonType, string? reasonText)
    {
        var res = ToBaseDto<ViewDto>();

        res.ReasonType = reasonType;
        res.ReasonText = reasonText;

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
        /// ReasonType
        /// </summary>
        [JsonIgnore]
        public ReasonType ReasonType { get; set; }

        /// <summary>
        /// ReasonTypeName
        /// </summary>
        public string ReasonTypeName => ReasonType.ToString();

        /// <summary>
        /// ReasonText
        /// </summary>
        public string? ReasonText { get; set; }

        #endregion
    }

    #endregion
}
