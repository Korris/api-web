namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Dtos;

partial class SocialPostReport
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SocialPostReport()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="postId">PostId</param>
    /// <param name="userId">UserId</param>
    /// <param name="reasonType">ReasonType</param>
    /// <param name="reasonText">ReasonText</param>
    /// <returns>Return the result</returns>
    public static SocialPostReport Create(Guid postId, Guid userId, ReasonType reasonType, string? reasonText)
    {
        var res = new SocialPostReport
        {
            PostId = postId,
            UserId = userId,
            ReasonType = reasonType,
            ReasonText = reasonText
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
            UserId = UserId,
            ReasonType = ((ReasonType)ReasonType).ToString(),
            ReasonText = ReasonText
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
        /// UserId
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// ReasonType
        /// </summary>
        public string? ReasonType { get; set; }

        /// <summary>
        /// ReasonText
        /// </summary>
        public string? ReasonText { get; set; }

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