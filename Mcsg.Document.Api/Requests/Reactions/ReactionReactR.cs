namespace Mcsg.Document.Api.Requests;

using Common.Core.Enums;
using Common.Core.Requests;

public class ReactionReactR : IdBaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ReactionReactR() { }

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="hc">HTTP context</param>
    public ReactionReactR(HttpContext hc) : base(hc) { }

    #endregion

    #region -- Properties --

    public Guid TargetId { get; set; }

    public ReactionType Type { get; set; }

    public bool? IsReply { get; set; }

    #endregion
}
