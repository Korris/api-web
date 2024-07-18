# Add migration
Add-Migration InitData -Args "host=localhost;port=5432;database=local_bumcheo;username=local;password=Local+54321z@"

# Update migration
CREATE DATABASE local_bumcheo_sync OWNER local;
Update-Database InitData -Args "host=ntata.postgres;port=5432;database=local_bumcheo_sync;username=local;password=Local+54321z@"
