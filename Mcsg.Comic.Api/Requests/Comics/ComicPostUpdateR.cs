namespace Mcsg.Comic.Api.Requests;

/// <summary>
/// Request
/// </summary>
public class ComicPostUpdateR : ComicPostFormBaseR
{
    public bool IsCompleted { get; set; }
}
