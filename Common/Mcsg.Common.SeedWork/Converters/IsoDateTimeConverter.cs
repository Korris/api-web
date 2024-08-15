using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mcsg.Common.SeedWork.Converters;

/// <summary>
/// IsoDateTime converter
/// </summary>
public class IsoDateTimeConverter : JsonConverter<DateTime>
{
    #region -- Overrides --

    /// <summary>
    /// Write
    /// </summary>
    /// <param name="writer">Writer</param>
    /// <param name="value">Value</param>
    /// <param name="options">Options</param>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ"));
    }

    /// <summary>
    /// Read
    /// </summary>
    /// <param name="reader">Reader</param>
    /// <param name="typeToConvert">Type to convert</param>
    /// <param name="options">Options</param>
    /// <returns>Returns the result</returns>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var t = reader.GetString();
        if (string.IsNullOrEmpty(t))
        {
            return DateTime.UtcNow;
        }

        return DateTime.Parse(t);
    }

    #endregion
}
