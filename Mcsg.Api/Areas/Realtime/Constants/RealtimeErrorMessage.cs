namespace Mcsg.Api.Areas.Realtime.Constants;

public static class RealtimeErrorMessage
{
    public const string InvalidRequest = "Request is invalid";
    public const string InvalidPostId = "Post is invalid";
    public const string InvalidReplyToCommentId = "Reply Comment is invalid";
    public const string InvalidMessage = "Message can not be empty";
    public const string NotFoundComment = "Comment not found";
    public const string NotFoundReply = "Reply not found";
    public const string UnAuthorizeUpdate = "User can modify their comment only";
}
