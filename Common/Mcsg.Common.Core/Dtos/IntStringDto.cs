namespace Mcsg.Common.Core.Dtos;

/// <summary>
/// IntString data transfer object
/// </summary>
public class IntStringDto
{
    #region -- Properties --

    /// <summary>
    /// Key
    /// </summary>
    public int Key { get; set; }

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; set; } = string.Empty;

    #endregion
}
