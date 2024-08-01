namespace Mcsg.Common.Domain.Entities;

using Core.Interfaces;
using Core.Rules;
using SeedWork.Dtos;
using SeedWork.Extensions;
using static Core.Constants.Setting;

partial class UserNameHistory
{
    #region -- Methods --

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="uniquenessChecker">uniquenessChecker</param>
    /// <param name="createdBy">Created by</param>
    /// <returns>Return the result</returns>
    public static UserNameHistory Create(IUniquenessChecker uniquenessChecker, Guid createdBy)
    {
        // Begin 2 number
        var headLength = 2;
        var tailLength = Default.FreeUserNameLength - headLength;
        var head = headLength.GenerateOtp();

        // Generate a new username
        var userName = head + tailLength.GetRandomString();
        var rule = new MustBeUniqueRule(uniquenessChecker, userName);

        // Check for duplicate usernames, then generate a new username and check again in the database to ensure it is unique
        while (rule.IsBroken())
        {
            userName = head + tailLength.GetRandomString();
            rule = new MustBeUniqueRule(uniquenessChecker, userName);
        }

        return new UserNameHistory
        {
            UserId = createdBy,
            UserName = userName,
            CreatedBy = createdBy
        };
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
            //TODO
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : DevNameDto
    {
        #region -- Properties --

        //TODO

        #endregion
    }

    /// <summary>
    /// Search
    /// </summary>
    public class SearchDto : BaseDto
    {
        #region -- Properties --

        //TODO

        #endregion
    }

    /// <summary>
    /// View
    /// </summary>
    public class ViewDto : BaseDto
    {
        #region -- Properties --

        //TODO

        #endregion
    }

    #endregion
}
