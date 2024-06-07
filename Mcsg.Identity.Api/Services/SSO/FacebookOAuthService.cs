using Mcsg.Lib.Common.Web.Security;
using Mcsg.Identity.Api.DTOs.Response.SSO;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Data.Repositories.Interface;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Mcsg.Identity.Api.Services.SSO
{
    public class FacebookOAuthService : SSOService
    {
        public FacebookOAuthService(ILogger<FacebookOAuthService> logger
            , IUnitOfWork unitOfWork
            , ISecurityService securityService) : base(logger, unitOfWork, securityService)
        {
        }

        public override async Task<SocialTokenResponse> VerifyToken(string socialToken)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://graph.facebook.com");
                    var queryParams = $"fields=id,first_name,last_name,name,email,gender,birthday&access_token={socialToken}";
                    var response = await client.GetAsync($"me?{queryParams}");
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        var content = await response.Content.ReadAsStringAsync();

                        var result = JsonConvert.DeserializeObject<FacebookProfileModel>(content, serializerSettings);
                        var claims = new List<Claim>
                        {
                        new Claim("Id", result.Id),
                        new Claim("Name", result.Name ?? string.Empty),
                        new Claim("GivenName", result.First_name ?? string.Empty),
                        new Claim("FamilyName", result.Last_name ?? string.Empty)
                        };

                        List<string> error = new List<string>();
                        if (string.IsNullOrEmpty(result.Id))
                        {
                            //error.Add(ErrorMessage.SocialIdNotPublic);
                        }

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
                                Id = result.Id ?? "",
                                Email = result.Email ?? "",
                                FirstName = result.First_name ?? "",
                                LastName = result.Last_name ?? "",
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
            }
            catch (Exception ex)
            {
                _logger.LogError($"Verify Facebook Token Exception: {ex.Message}", socialToken);
                throw new BadRequestException(ErrorCodes.ApiErrorCode, ex.Message);
            }
        }
    }
}
