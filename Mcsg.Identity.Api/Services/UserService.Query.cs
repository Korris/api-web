namespace Mcsg.Identity.Api.Services
{
    public partial class UserService
    {
        private string UpdateEmailConfirmedCommand
        {
            get
            {
                return @"UPDATE {_userRepository.TableName}
                        SET ""Email"" = @email, 
                            ""EmailConfirmed"" = true
                        WHERE ""Id"" = @id AND ""EmailConfirmed"" = false";
            }
        }

        private string UpdatePhoneNumberConfirmedCommand
        {
            get
            {
                return @$"UPDATE {_userRepository.TableName}
                        SET ""PhoneNumber"" = @phone, 
                            ""PhoneNumberConfirmed"" = true
                        WHERE ""Id"" = @id AND ""PhoneNumberConfirmed"" = false";
            }
        }
    }
}
