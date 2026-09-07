namespace Mcsg.Api.Areas.Document.Requests;

/// <summary>
/// Request
/// </summary>
public class DocumentPostUpdateR : DocumentPostFormBaseR
{
    public bool IsCompleted { get; set; }
}
