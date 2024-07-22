# Add migration
Add-Migration InitData -Args "host=localhost;port=5432;database=local_bumcheo_wallet;username=local;password=Local+54321z@"

# Update migration
CREATE DATABASE local_bumcheo_wallet OWNER local;
Update-Database InitData -Args "host=localhost;port=5432;database=local_bumcheo_wallet;username=local;password=Local+54321z@"
