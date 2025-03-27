namespace Mcsg.Common.Domain.Entities;

using SeedWork.Dtos;

partial class SystemSetting
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public SystemSetting()
    {
        IsActive = true;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="value"></param>
    /// <param name="dataType"></param>
    /// <param name="modifiedBy"></param>
    /// <returns></returns>
    public void Update(string? value, string? dataType, Guid modifiedBy)
    {
        Value = value;
        DataType = (dataType + "").ToLower();

        ModifiedOn = DateTime.UtcNow;
        ModifiedBy = modifiedBy;
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
            Key = Key,
            Value = Value,
            DataType = DataType,
        };
    }

    #endregion

    #region -- Classes --

    /// <summary>
    /// Base
    /// </summary>
    public class BaseDto : IdDto
    {
        /// <summary>
        /// Key
        /// </summary>
        public string? Key { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// DataType
        /// </summary>
        public string? DataType { get; set; }
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