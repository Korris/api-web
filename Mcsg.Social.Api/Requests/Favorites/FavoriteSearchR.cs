namespace Mcsg.Social.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class FavoriteSearchR : PagingR
{
    #region -- Properties --

    /// <summary>
    /// ServiceType
    /// </summary>
    public string? ServiceType { get; set; }

    #endregion
}
