namespace Mcsg.Common.Domain.Entities;

using SeedWork.Dtos;

partial class UserReferral
{
    #region -- Methods --

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="userReferrerId">User Referrer</param>
    /// <param name="createdBy">Created by</param>
    /// <returns>Return the result</returns>
    public static UserReferral Create(Guid userReferrerId, Guid createdBy)
    {
        return new UserReferral
        {
            UserReferrerId = userReferrerId,
            UserRefereeId = createdBy,
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
            Id = Id,
            UserReferee = new UserDto
            {
                Id = UserRefereeId,
                ProfileName = UserReferee?.ProfileName,
                UserAvatar = UserReferee?.Avatar,
                UserName = UserReferee?.UserName
            },
            CreatedOn = CreatedOn
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
        /// UserReferee
        /// </summary>
        public UserDto? UserReferee { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }

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

    /// <summary>
    /// User
    /// </summary>
    public class UserDto : IdDto
    {
        #region -- Properties --

        /// <summary>
        /// User avatar
        /// </summary>
        public string? UserAvatar { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Profile name
        /// </summary>
        public string? ProfileName { get; set; }

        #endregion
    }

    #endregion
}
