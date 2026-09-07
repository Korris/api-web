namespace Mcsg.Api.Areas.Document.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class ResourceViewR : BaseR
{
    public string? PostHashId { get; set; }

    public float? Order { get; set; }
}
