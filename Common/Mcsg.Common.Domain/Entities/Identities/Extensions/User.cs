using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using Core;
using Core.Dtos;
using SeedWork.Dtos;
using SeedWork.Enums;

partial class User
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public User()
    {
        Id = Guid.NewGuid();
        Type = UserType.Guest;
        CreatedOn = DateTime.UtcNow;
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

    /// <summary>
    /// Create JWT
    /// </summary>
    /// <param name="sessionId">SessionId</param>
    /// <param name="jwt">JWT setting</param>
    /// <param name="roles">Roles</param>
    /// <returns>Returns the result</returns>
    public TokenDto CreateJwt(Guid sessionId, JwtDto jwt, string? roles)
    {
        var payload = new PayloadDto
        {
            Id = Id,
            UserName = UserName + "",
            ProfileName = ProfileName + "",
            ProfileId = ProfileId + "",
            UserFolder = UserFolder,
            UserAvatar = Avatar + "",
            IsPremium = IsPremium,
            IsWalletShowing = IsWalletShowing,
            SessionId = sessionId,
            MinioInstance = MinioInstance,
            StorageLimit = StorageLimit
        };

        if (!string.IsNullOrWhiteSpace(roles))
        {
            payload.Roles = roles.Split(",");
        }

        var st = new SecurityToken(jwt, payload);
        var delay = 30; // time delay between server and client (seconds)

        return new TokenDto
        {
            AccessToken = st.Jwt,
            ExpiredDate = st.ExpiredDate.AddSeconds(-delay)
        };
    }

    #endregion

    #region -- Properties --

    [InverseProperty("UserReferee")]
    [NotMapped]
    public virtual ICollection<UserReferral> UserReferralUserReferees { get; set; } = [];

    [InverseProperty("UserReferrer")]
    [NotMapped]
    public virtual ICollection<UserReferral> UserReferralUserReferrers { get; set; } = [];

    /// <summary>
    /// Host
    /// </summary>
    [NotMapped]
    public bool IsPremium => PremiumDate != null && PremiumDate > DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// UserFolder on MinIO
    /// </summary>
    [NotMapped]
    public string UserFolder => ProfileId + "";

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : DevNameDto
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

    /// <summary>
    /// Profile
    /// </summary>
    public class ProfileDto : IdDto
    {
        #region -- Properties --

        /// <summary>
        /// UserName
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// ProfileName
        /// </summary>
        public string? ProfileName { get; set; }

        #endregion
    }

    #endregion
}
