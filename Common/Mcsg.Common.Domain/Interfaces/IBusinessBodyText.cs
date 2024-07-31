namespace Mcsg.Common.Domain.Interfaces;

/// <summary>
/// Marker interface to represent a business body text
/// </summary>
public interface IBusinessBodyText
{
    #region -- Methods --

    /// <summary>
    /// Process
    /// </summary>
    /// <param name="bodyText">Body text</param>
    /// <returns>Return the result</returns>
    Task<string?> Process(string? bodyText);

    #endregion
}
