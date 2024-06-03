using Mcsg.Lib.Data.Enums;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Mcsg.Identity.Api.Services
{
    public partial class OtpService
    {
        private string GetValidTokenQuery
        {
            get
            {
                return @$"SELECT ""Code"" FROM {_userOtpRepository.TableName}
                      WHERE ""Token"" = @Token AND ""OtpType"" = @Type and ""ExpiryTime"" > now() AT TIME ZONE 'UTC'";
            }
        }

        private string DeleteUserOtpsQuery
        {
            get
            {
                return  $"DELETE FROM {_userOtpRepository.TableName} WHERE \"UserId\" = @UserId AND \"OtpType\" = @Type ";
            }
        }
        private string GetOtpQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""Code"", ""UserId"", ""OtpType"", ""Destination"" FROM {_userOtpRepository.TableName}
                      WHERE ""Token"" = @Token AND ""OtpType"" = @Type ";
            }
        }
        private string GetValidOtpQuery
        {
            get
            {
                return @$"SELECT ""Id"", ""Code"", ""UserId"", ""OtpType"", ""Destination"" FROM {_userOtpRepository.TableName}
                      WHERE ""Token"" = @Token AND ""OtpType"" = @Type and ""ExpiryTime"" > now() AT TIME ZONE 'UTC'";
            }
        }
    }
}
