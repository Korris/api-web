# Add migration
Add-Migration InitData -Args "host=ntata.postgres;port=5432;database=local_bumcheo_wallet;username=local;password=Local+54321z@"

# Update migration
Update-Database InitData -Args "host=ntata.postgres;port=5432;database=local_bumcheo_wallet;username=local;password=Local+54321z@"
