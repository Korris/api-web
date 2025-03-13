# Add migration
Set as Startup Project: Common\Mcsg.Common.Domain
Tools -> Nuget Package Manager -> Package Manager Console
Default project: Common\Mcsg.Common.Domain
Run command below:
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL -Version 8.0.11
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.14
Add-Migration InitData -Args "host=localhost;port=5433;database=local_focfoc;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Common.Domain.csproj file

# Update migration
```
-- Terminate all active connections to the database
SELECT pg_terminate_backend(pg_stat_activity.pid)
FROM pg_stat_activity
WHERE pg_stat_activity.datname = 'local_focfoc'
AND pid <> pg_backend_pid();

-- Drop the database
DROP DATABASE local_focfoc;

-- Recreate the database with a specific owner
CREATE DATABASE local_focfoc OWNER local;
```
Install-Package Npgsql.EntityFrameworkCore.PostgreSQL -Version 8.0.11
Install-Package Microsoft.EntityFrameworkCore.Tools -Version 8.0.14
Update-Database InitData -Args "host=ntada.win;Port=65432;database=local_focfoc;username=local;password=Local+54321z@"
Note:
Revert Mcsg.Common.Domain.csproj file
