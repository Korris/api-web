# Add migration
Set as Startup Project: Lib\Mcsg.Wallet.Domain
Tools -> Nuget Package Manager -> Package Manager Console
Default project: Lib\Mcsg.Wallet.Domain
Run command below:
Install-Package Microsoft.EntityFrameworkCore
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
Install-Package Microsoft.EntityFrameworkCore.Tools
Add-Migration InitData -Args "host=localhost;port=5433;database=local_bumcheo_wallet;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Wallet.Domain.csproj file

# Update migration
```
-- Terminate all active connections to the database
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'local_bumcheo_wallet'
AND pid <> pg_backend_pid();

-- Drop the database
DROP DATABASE local_bumcheo_wallet;

-- Recreate the database with a specific owner
CREATE DATABASE local_bumcheo_wallet OWNER local;
```
Install-Package Microsoft.EntityFrameworkCore
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
Install-Package Microsoft.EntityFrameworkCore.Tools
Update-Database InitData -Args "host=ntada.win;Port=65433;database=local_bumcheo_wallet;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Wallet.Domain.csproj file
