namespace Mcsg.Api.Areas.TapShow.Requests;

using Common.Core.Requests;

/// <summary>
/// Check that a TapShow post still exists (by PostId or PostHashId)
/// </summary>
public class CommentCheckPostExistedR : BaseR
{
    public Guid? PostId { get; set; }
    public string? PostHashId { get; set; }
}
