namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
using SeedWork.Constants;
using SeedWork.Dtos;

partial class Feedback
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public Feedback()
    {
        Id = Guid.NewGuid();
        CreatedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="feedbackType"></param>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static Feedback Create(FeedbackType feedbackType, Guid? userId, string email, string comment)
    {
        var res = new Feedback
        {
            Type = feedbackType,
            UserId = userId,
            Email = email,
            Comment = comment,
            CreatedBy = userId == null ? Setting.CreatedBy.System : userId
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