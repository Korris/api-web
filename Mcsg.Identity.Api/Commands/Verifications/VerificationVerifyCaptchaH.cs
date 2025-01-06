using MediatR;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class VerificationVerifyCaptchaH : BaseSettingH, IRequestHandler<VerificationVerifyCaptchaR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public VerificationVerifyCaptchaH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(VerificationVerifyCaptchaR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new VerificationVerifyCaptchaV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            return res.SetError(nameof(E000), E000, t);
        }

        var secret = _setting.ReCaptchaSecretKey;
        var url = $"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={request.Token}";

        using var httpClient = new HttpClient();
        var rsp = await httpClient.GetAsync(url, cancellationToken);

        if (!rsp.IsSuccessStatusCode)
        {
            "Error verifying reCAPTCHA".LogError();
            return res.SetError(nameof(E102), E102);
        }

        return res;
    }

    #endregion
}
