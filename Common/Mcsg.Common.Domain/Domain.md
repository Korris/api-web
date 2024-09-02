# Add migration
Set as Startup Project: Common\Mcsg.Common.Domain
Tools -> Nuget Package Manager -> Package Manager Console
Default project: Common\Mcsg.Common.Domain
Run command below:
Install-Package Microsoft.EntityFrameworkCore
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
Install-Package Microsoft.EntityFrameworkCore.Tools
Add-Migration InitData -Args "host=localhost;port=5433;database=local_bumcheo;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Common.Domain.csproj file

# Update migration
```
-- Terminate all active connections to the database
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'local_bumcheo'
AND pid <> pg_backend_pid();

-- Drop the database
DROP DATABASE local_bumcheo;

-- Recreate the database with a specific owner
CREATE DATABASE local_bumcheo OWNER local;
```
Install-Package Microsoft.EntityFrameworkCore
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL
Install-Package Microsoft.EntityFrameworkCore.Tools
Update-Database InitData -Args "host=ntata.postgres;port=5433;database=local_bumcheo;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Common.Domain.csproj file
