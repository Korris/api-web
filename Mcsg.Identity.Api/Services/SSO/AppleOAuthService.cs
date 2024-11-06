using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services;

using Common.Core.Constants;
using Common.Core.Interfaces;
using Common.SeedWork.Constants;
using Common.SeedWork.Exceptions;
using Response;

public class AppleOAuthService : SSOService
{
    public AppleOAuthService(ILogger<AppleOAuthService> logger, ISecurityService securityService) : base(logger, securityService) { }

    public override async Task<SocialTokenResponse> VerifyToken(string socialToken)
    {
        try
        {
            //return await Task.FromResult(new SocialTokenResponse());

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://appleid.apple.com");

                var response = await client.GetAsync("/auth/keys");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var applePublicKeysResponse = JsonConvert.DeserializeObject<ApplePublicKeysResponse>(content, serializerSettings);
                    string[] tokenPart = socialToken.Split('.');
                    JObject tokenHeaderObject = JsonConvert.DeserializeObject<JObject>(_securityService.FromBase64ToString(tokenPart[0]), serializerSettings);
                    var publicKey = applePublicKeysResponse?.Keys.FirstOrDefault(x => x.Kid == tokenHeaderObject?.GetValue("kid")?.ToString() && x.Alg == tokenHeaderObject?.GetValue("alg")?.ToString());
                    if (publicKey == null)
                    {
                        _logger.LogError($" Verify Apple Token Error [cant get public key]: ", new { Request = socialToken, applePublicKeysResponse = applePublicKeysResponse });
                        return null;// cant get public key
                    }

                    bool verifySuccess = _securityService.RsaVerifySignature(socialToken, publicKey.N, publicKey.E);

                    if (!verifySuccess)
                    {
                        _logger.LogError($" Verify Apple Token Error [have problem with token]: ", new { Request = socialToken, publicKey = publicKey });
                        return null;// have problem with token
                    }
                    var result = JsonConvert.DeserializeObject<AppleProfileModel>(_securityService.FromBase64ToString(tokenPart[1]), serializerSettings);

                    if (result == null)
                    {
                        _logger.LogError($" Verify Apple Token Error [have problem with token]: ", new { Request = socialToken, publicKey = publicKey });
                        return null; // have problem with token
                    }
                    List<string> error = new List<string>();
                    if (string.IsNullOrEmpty(result.Sub))

                    {
                        error.Add(ErrorMessage.SocialIdNotPublic);
                    }

                    var claims = new List<Claim>
                    {
                        new Claim("Id", result.Sub),
                        new Claim("Name", result.Given_name ?? string.Empty),
                        new Claim("GivenName", result.Given_name ?? string.Empty),
                        new Claim("FamilyName", result.Family_name ?? string.Empty)
                    };

                    if (!string.IsNullOrEmpty(result.Email))
                    {
                        claims.Add(new Claim("Email", result.Email ?? string.Empty));
                    }

                    return new SocialTokenResponse()
                    {
                        Claims = claims,
                        Error = error,
                        Profile = new SocialProfileModel()
                        {
                            Id = result.Sub ?? "",
                            Email = result.Email ?? "",
                            FirstName = result.Given_name ?? "",
                            LastName = result.Family_name ?? "",
                        }
                    };
                }
                else
                {
                    _logger.LogError("Verify Apple Token Error [get list key from apple]: " +
                        new { Request = socialToken, Response = await response.Content.ReadAsStringAsync() });
                    return null;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Verify Apple Token Exception: {ex.Message}", socialToken);
            throw new BadRequestException(Error.E500, ex.Message);
        }
    }
}
