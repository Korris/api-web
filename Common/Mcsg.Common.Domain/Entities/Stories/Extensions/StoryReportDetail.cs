namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Dtos;

partial class StoryReportDetail
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StoryReportDetail()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="userId"></param>
    /// <param name="reasonType"></param>
    /// <param name="reasonText"></param>
    /// <returns>Return the result</returns>
    public static StoryReportDetail Create(Guid reportId, Guid userId, ReasonType reasonType, string reasonText)
    {
        var res = new StoryReportDetail
        {
            ReportId = reportId,
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
            ReportId = ReportId,
            UserId = UserId,
            ReasonType = ((ReasonType)ReasonType).ToString(),
            ReasonText = ReasonText,
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
        /// ReportId
        /// </summary>
        public Guid? ReportId { get; set; }

        /// <summary>
        /// UserId
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Reason type
        /// </summary>
        public string? ReasonType { get; set; }

        /// <summary>
        /// Reason text
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