namespace Mcsg.Admin.Api.Services
{
    public partial class SystemSettingService
    {
        private string ActiveSystemSettingQuery
        {
            get
            {
                return $"SELECT \"Id\", \"Key\", \"Value\" FROM {_systemSettingRepo.TableName} WHERE \"Key\" = @Key AND \"IsActive\" = @IsActive;";
            }
        }
    }
}
