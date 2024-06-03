using MC_SG.Lib.Common.Web.Security;
using Mcsg.Identity.Api.DTOs.Response.SSO;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Data.Repositories.Interface;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services.SSO
{
    public class GoogleOAuthService : SSOService
    {
        public GoogleOAuthService(ILogger<GoogleOAuthService> logger
            , IUnitOfWork unitOfWork
            , ISecurityService securityService) : base(logger, unitOfWork, securityService)
        {
        }

        public override async Task<SocialTokenResponse> VerifyToken(string socialToken)
        {
            var client = new HttpClient();
            try
            {
                client.BaseAddress = new Uri("https://www.googleapis.com");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", socialToken);
                var response = await client.GetAsync("/oauth2/v1/userinfo");

                if (!response.IsSuccessStatusCode)
                {
                    client.Dispose();
                    client = new HttpClient();
                    // try another auth link, issue from firebase
                    client.BaseAddress = new Uri("https://oauth2.googleapis.com");
                    response = await client.GetAsync($"tokeninfo?id_token={socialToken}");
                }
                if (response.IsSuccessStatusCode)
                {
                    // Read the response content as a string
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<GoogleProfileModel>(content, serializerSettings);
                    var claims = new List<Claim>
                            {
                                new Claim("Id", result.Id?? string.Empty),
                                new Claim("Name", result.Name ?? string.Empty),
                                new Claim("GivenName", result.Given_name  ?? string.Empty),
                                new Claim("FamilyName", result.Family_name ?? string.Empty),
                                new Claim("Email", result.Email ?? string.Empty)
                            };

                    List<string> error = new List<string>();
                    if (string.IsNullOrEmpty(result.Id))
                    {
                        claims.Remove(claims.FirstOrDefault(x => x.Type == "Id"));
                        claims.Add(new Claim("Id", result.Email ?? string.Empty));
                    }

                    return new SocialTokenResponse()
                    {
                        Claims = claims,
                        Error = error,
                        Profile = new SocialProfileModel()
                        {
                            Id = result.Id ?? "",
                            Email = result.Email ?? "",
                            FirstName = result.Given_name ?? "",
                            LastName = result.Family_name ?? "",
                        }
                    };
                }
                else
                {
                    // Handle the error or log it
                    _logger.LogError($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                    return null;
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Verify Google Token Exception: {ex.Message}", socialToken);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }
            finally
            {
                client.Dispose();
            }
        }
    }
}
