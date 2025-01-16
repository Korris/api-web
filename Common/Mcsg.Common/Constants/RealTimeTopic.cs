namespace Mcsg.Common.Constants;

public static class RealTimeTopic
{
    #region Comment
    public const string ReceiveComment = "ReceiveComment";
    public const string ReceiveUpdateComment = "ReceiveUpdateComment";
    public const string ReceiveDeleteComment = "ReceiveDeleteComment";
    public const string ReceiveReply = "ReceiveReply";
    public const string ReceiveUpdateReply = "ReceiveUpdateReply";
    public const string ReceiveDeleteReply = "ReceiveDeleteReply";
    #endregion

    #region Notification
    public const string ReceiveNotification = "ReceiveNotification";
    public const string ReceiveTransactionUpdate = "ReceiveTransactionUpdate";
    public const string ReceiveSendSuccessComment = "ReceiveSendSuccessComment";
    public const string ReceiveForceLogout = "ReceiveForceLogout";
    public const string ReceiveVerifyOtp = "ReceiveVerifyOtp";
    public const string ReceiveDepositSucces = "ReceiveDepositSucces";
    #endregion

    #region Follow
    public const string ReceiveFollow = "ReceiveFollow";
    public const string ReceiveFollowPost = "ReceiveFollowPost";
    #endregion
}
