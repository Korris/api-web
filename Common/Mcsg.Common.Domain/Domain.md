# Add migration
Add-Migration InitData -Args "host=localhost;port=5432;database=local_bumcheo;username=local;password=Local+54321z@"

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
Update-Database InitData -Args "host=ntata.postgres;port=5432;database=local_bumcheo;username=local;password=Local+54321z@"
