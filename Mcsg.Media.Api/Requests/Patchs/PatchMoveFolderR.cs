namespace Mcsg.Media.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PatchMoveFolderR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchMoveFolderR()
    {
        Otp = string.Empty;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// OTP
    /// </summary>
    public string Otp { get; set; }

    #endregion
}