# Add migration
Add-Migration InitData -Args "host=localhost;port=5432;database=local_bumcheo;username=local;password=Local+54321z@"

# Update migration
CREATE DATABASE local_bumcheo OWNER local;
Update-Database InitData -Args "host=localhost;port=5432;database=local_bumcheo;username=local;password=Local+54321z@"
