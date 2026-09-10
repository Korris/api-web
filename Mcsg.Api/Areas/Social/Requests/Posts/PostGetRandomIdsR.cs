namespace Mcsg.Api.Areas.Social.Requests;

using Common.Core.Requests;

/// <summary>
/// Request for getting random post ids
/// </summary>
public class PostGetRandomIdsR : BaseR
{
    /// <summary>
    /// Is have media
    /// </summary>
    public bool IsHaveMedia { get; set; }

    /// <summary>
    /// ExcludeHashId
    /// </summary>
    public string? ExcludeHashId { get; set; }
}
