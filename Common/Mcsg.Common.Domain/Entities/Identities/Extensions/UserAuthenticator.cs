namespace Mcsg.Common.Domain.Entities;

using SeedWork.Dtos;

partial class UserAuthenticator
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserAuthenticator()
    {
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="secretKey">Secret key</param>
    /// <param name="createdBy">Created by</param>
    /// <returns>Return the result</returns>
    public static UserAuthenticator Create(string secretKey, Guid createdBy)
    {
        var res = new UserAuthenticator
        {
            UserId = createdBy,
            Secretkey = secretKey,
            CreatedBy = createdBy
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="secretKey">Secret key</param>
    /// <param name="modifiedBy">Modified by</param>
    public void Update(string secretKey, Guid modifiedBy)
    {
        Secretkey = secretKey;
        IsDelete = false;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Update (active 2FA)
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Update(Guid modifiedBy)
    {
        IsActive = true;
        IsLogin = true;
        IsTransaction = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Update (enable/disable for Login or Transaction)
    /// </summary>
    /// <param name="isLogin"></param>
    /// <param name="isTransaction"></param>
    /// <param name="modifiedBy"></param>
    public void Update(bool? isLogin, bool? isTransaction, Guid modifiedBy)
    {
        IsLogin = isLogin ?? false;
        IsTransaction = isTransaction ?? false;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Delete(Guid modifiedBy)
    {
        IsActive = false;
        IsLogin = false;
        IsTransaction = false;
        IsDelete = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
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
        #region -- Properties --

        /// <summary>
        /// QrCode
        /// </summary>
        public string QrCode { get; set; } = default!;

        /// <summary>
        /// SecretKey
        /// </summary>
        public string SecretKey { get; set; } = default!;

        #endregion
    }

    #endregion
}
