namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Dtos;

partial class DocumentReportDetail
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentReportDetail()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="reportId"></param>
    /// <param name="reasonType"></param>
    /// <param name="reasonText"></param>
    /// <param name="createdBy"></param>
    /// <returns>Return the result</returns>
    public static DocumentReportDetail Create(Guid reportId, ReasonType reasonType, string? reasonText, Guid createdBy)
    {
        var res = new DocumentReportDetail
        {
            ReportId = reportId,
            ReasonType = reasonType,
            ReasonText = reasonText,
            UserId = createdBy,
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
    }

    #endregion
}
