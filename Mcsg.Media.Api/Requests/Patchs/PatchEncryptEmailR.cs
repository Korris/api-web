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

    /// <summary>
    /// Gets or sets a value indicating whether the content has been decrypted.
    /// </summary>
    public bool IsDecrypted { get; set; }

    #endregion
}