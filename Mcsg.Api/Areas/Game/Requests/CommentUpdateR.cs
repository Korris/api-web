namespace Mcsg.Api.Areas.Game.Requests;

using Common.Core.Requests;

/// <summary>
/// Update comment body (Id from route)
/// </summary>
public class CommentUpdateR : IdBaseR
{
    public string? Body { get; set; }
}
