using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Common.Core.Extensions;
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
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto(string mediaApiUrl)
    {
        return ToBaseDto<SearchDto>(mediaApiUrl);
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto(string mediaApiUrl)
    {
        return ToBaseDto<ViewDto>(mediaApiUrl);
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <param name="mediaApiUrl">Media API URL</param>
    /// <returns>Return the DTO</returns>
    public T ToBaseDto<T>(string mediaApiUrl) where T : BaseDto, new()
    {
        return new T
        {
            Id = Id,
            UserReferrer = new UserDto
            {
                Id = UserReferrerId,
                ProfileName = UserReferrer?.ProfileName,
                UserAvatar = mediaApiUrl.ToPublicImageUrl(UserReferrer?.Avatar + ""),
                UserName = UserReferrer?.UserName
            },
            CreatedOn = CreatedOn
        };
    }

    #endregion

    #region -- Properties --

    [ForeignKey("UserRefereeId")]
    [InverseProperty("UserReferralUserReferees")]
    public virtual User UserReferee { get; set; } = null!;

    [ForeignKey("UserReferrerId")]
    [InverseProperty("UserReferralUserReferrers")]
    public virtual User UserReferrer { get; set; } = null!;

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : DevNameDto
    {
        #region -- Properties --

        /// <summary>
        /// UserReferrer
        /// </summary>
        public UserDto? UserReferrer { get; set; }

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
