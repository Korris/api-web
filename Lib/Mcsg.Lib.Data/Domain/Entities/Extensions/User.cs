using System.ComponentModel.DataAnnotations.Schema;

namespace Mcsg.Lib.Data.Domain.Entities;

using Mcsg.Common.SeedWork.Dtos;
using Mcsg.Common.SeedWork.Enums;

public partial class User
{
    #region -- Methods --

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

    /// <summary>
    /// Host
    /// </summary>
    [NotMapped]
    public UserType Type
    {
        get
        {
            var dateNow = DateOnly.FromDateTime(DateTime.UtcNow);
            return PremiumDate >= dateNow ? UserType.Premium : UserType.Free;
        }
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
