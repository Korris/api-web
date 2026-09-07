namespace Mcsg.Api.Areas.Story.Requests;

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
