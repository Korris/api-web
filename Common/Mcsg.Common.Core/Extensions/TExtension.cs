#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mcsg.Common.Core.Extensions;

/// <summary>
/// T extension for using [this T] only
/// </summary>
public static class TExtension
{
    #region -- Methods --

    /// <summary>
    /// Converts an object to a JSON string.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="o">The object to serialize.</param>
    /// <param name="camelCase">Whether to use camel case for property names.</param>
    /// <returns>The JSON string representation of the object.</returns>
    public static string ToJson<T>(this T o, bool camelCase = false)
    {
        if (o == null)
            return string.Empty;

        var settings = camelCase
            ? new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() }
            : null;

        return JsonConvert.SerializeObject(o, settings);
    }

    #endregion
}
