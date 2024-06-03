# Add migration
Add-Migration Add-Earning-Summary -OutputDir "Migrations/202404" -Args "host=localhost;port=5432;database=mcsg;username=postgres;password=postgres"

# Update migration local
Update-Database -Args "host=localhost;port=5432;database=mcsg;username=postgres;password=postgres"
"Server=dev-mcsg-dbserver.postgres.database.azure.com;Database=devmcsgdb;Port=5432;User Id=mcsgdbadmin;Password=MyPassWord@123;"
Get-Migration  -Args "host=dev-mcsg-dbserver.postgres.database.azure.com;port=5432;database=devmcsgdb;username=mcsgdbadmin;password=MyPassWord@123"
Remove-Migration -Args "host=localhost;port=5432;database=mcsg;username=postgres;password=postgres"
# Drop db
DROP SCHEMA public CASCADE;
CREATE SCHEMA public;

# Admin account
admin/MyPassword@123
