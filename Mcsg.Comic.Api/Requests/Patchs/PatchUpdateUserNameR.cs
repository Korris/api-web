namespace Mcsg.Comic.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PatchUpdateUserNameR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchUpdateUserNameR()
    {
        Otp = string.Empty;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// OTP
    /// </summary>
    public string? Otp { get; set; }

    #endregion
}