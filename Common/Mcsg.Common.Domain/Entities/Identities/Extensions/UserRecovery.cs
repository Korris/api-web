namespace Mcsg.Common.Domain.Entities;

using Common.SeedWork;
using SeedWork.Dtos;

partial class UserRecovery
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserRecovery()
    {
    }

    /// <summary>
    /// Create
    /// </summary>
    /// <param name="secretKey">Secret key</param>
    /// <param name="createdBy">Created by</param>
    /// <returns>Return the result</returns>
    public static UserRecovery Create(string secretKey, Guid createdBy)
    {
        var res = new UserRecovery
        {
            UserId = createdBy,
            SecretKey = secretKey,
            CreatedBy = createdBy
        };

        return res;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Update(Guid modifiedBy)
    {
        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Delete
    /// </summary>
    /// <param name="modifiedBy"></param>
    public void Delete(Guid modifiedBy)
    {
        IsDelete = true;

        ModifiedBy = modifiedBy;
        ModifiedOn = DateTime.UtcNow;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <param name="aes">Security aes</param>
    /// <returns>Return the DTO</returns>
    public SearchDto ToSearchDto(ISecurityAes aes)
    {
        var res = ToBaseDto<SearchDto>();

        res.SecretKey = aes.DecryptText(SecretKey) + "";
        res.IsUsed = ModifiedOn != null;

        return res;
    }

    /// <summary>
    /// Convert to data transfer object
    /// </summary>
    /// <returns>Return the DTO</returns>
    public ViewDto ToViewDto(string secretKey)
    {
        var res = ToBaseDto<ViewDto>();

        res.SecretKey = secretKey;
        res.IsUsed = false;

        return res;
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
        #region -- Properties --

        /// <summary>
        /// SecretKey
        /// </summary>
        public string SecretKey { get; set; } = default!;

        /// <summary>
        /// SecretKey
        /// </summary>
        public bool IsUsed { get; set; }

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
