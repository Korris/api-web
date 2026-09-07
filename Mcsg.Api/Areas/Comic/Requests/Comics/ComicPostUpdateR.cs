namespace Mcsg.Api.Areas.Comic.Requests;

/// <summary>
/// Request
/// </summary>
public class ComicPostUpdateR : ComicPostFormBaseR
{
    public bool IsCompleted { get; set; }
}
