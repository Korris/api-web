namespace Mcsg.Common.Domain.Entities;

using Core.Enums;
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
    /// <param name="satisfactionLevel"></param>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <param name="comment"></param>
    /// <returns></returns>
    public static Feedback Create(SatisfactionLevel satisfactionLevel, Guid userId, string email, string? comment, PostType postType)
    {
        var res = new Feedback
        {
            Satisfaction = satisfactionLevel,
            UserId = userId,
            Email = email,
            Comment = comment + "",
            CreatedBy = userId,
            Type = postType,

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