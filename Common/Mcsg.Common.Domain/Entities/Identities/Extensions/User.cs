using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Common.Domain.Entities;

using SeedWork.Dtos;

partial class User
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public User()
    {
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

    #endregion
}
