namespace Mcsg.Api.Areas.Comic.Requests;

using Mcsg.Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class SubPostSearchR : PagingR
{
    #region -- Properties --

    /// <summary>
    /// Id
    /// </summary>
    public Guid PostId { get; set; }

    #endregion
}
