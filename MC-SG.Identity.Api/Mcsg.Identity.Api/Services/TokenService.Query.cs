using Mcsg.Lib.Data.Repositories;

namespace Mcsg.Identity.Api.Services
{
    public partial class TokenService
    {
        private string GetByRefreshTokenQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""RefreshTokenExpiryTime"", ""UserId"" FROM {_userRefreshTokenRepository.TableName}
                      WHERE ""RefreshToken"" = @RefreshToken ";
            }
        }
        private string GetRefreshTokenByUserIdQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""RefreshTokenExpiryTime"", ""RefreshToken"", ""UserId"" FROM {_userRefreshTokenRepository.TableName}
                      WHERE ""UserId"" = @UserId ORDER BY ""RefreshTokenExpiryTime"" DESC";
            }
        }

        private string DeleteRefreshTokenByUserIdCommand
        {
            get
            {
                return @$"DELETE FROM {_userRefreshTokenRepository.TableName}
                          WHERE ""UserId"" = @UserId";
            }
        }
    }
}
