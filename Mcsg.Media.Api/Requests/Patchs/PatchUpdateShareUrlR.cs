namespace Mcsg.Media.Api.Requests;

using Common.Core.Requests;

/// <summary>
/// Request
/// </summary>
public class PatchUpdateShareUrlR : BaseR
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchUpdateShareUrlR()
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