namespace Mcsg.Media.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PatchEncryptEmailR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchEncryptEmailR()
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