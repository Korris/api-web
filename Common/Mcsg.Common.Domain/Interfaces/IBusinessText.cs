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
    /// <param name="profiles">Profiles</param>
    /// <returns>Return the result</returns>
    Task<string> Process(string? text, List<Entities.User.ProfileDto>? profiles = null);

    /// <summary>
    /// Get profiles
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Return the result</returns>
    Task<List<Entities.User.ProfileDto>> GetProfiles(string? text);

    /// <summary>
    /// ConvertBodyFromMobile
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    Task<string> ConvertBodyFromMobile(string? text);

    /// <summary>
    /// ConvertCustomNoteFromMobile
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    string ConvertCustomNoteFromMobile(string? text);

    #endregion
}
