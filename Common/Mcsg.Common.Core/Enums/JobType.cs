namespace Mcsg.Common.Core.Enums;

/// <summary>
/// Job type
/// </summary>
public enum JobType
{
    /// <summary>
    /// VerifyByEmailOtp
    /// </summary>
    VerifyByEmailOtp,

    /// <summary>
    /// ResetByEmailOtp
    /// </summary>
    ResetByEmailOtp,

    /// <summary>
    /// ConfirmEmailOtp
    /// </summary>
    ConfirmEmailOtp,

    /// <summary>
    /// SmsOtp
    /// </summary>
    SmsOtp,

    /// <summary>
    /// ConvertVideo
    /// </summary>
    ConvertVideo,

    /// <summary>
    /// ConvertAudio
    /// </summary>
    ConvertAudio,

    /// <summary>
    /// MergeVideo
    /// </summary>
    MergeVideo,

    /// <summary>
    /// MergeAudioToVideo
    /// </summary>
    MergeAudioToVideo,

    /// <summary>
    /// MergeAudioToImage
    /// </summary>
    MergeAudioToImage,

    /// <summary>
    /// PaymentTransaction
    /// </summary>
    PaymentTransaction,

    /// <summary>
    /// WithDrawNoti
    /// </summary>
    WithDrawNoti,

    /// <summary>
    /// DepositNoti
    /// </summary>
    DepositNoti,
}