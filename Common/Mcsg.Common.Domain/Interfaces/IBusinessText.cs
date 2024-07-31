namespace Mcsg.Common.Domain;

/// <summary>
/// Marker interface to represent a business text
/// </summary>
public interface IBusinessText
{
    #region -- Methods --

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Return the result</returns>
    Task<string> Process(string? text);

    #endregion
}
