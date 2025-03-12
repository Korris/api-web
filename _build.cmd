@echo off

set Source=%WORKSPACE%
set Publish=%WORKSPACE%\..\_publish
set ZipFile=%WORKSPACE%\%3.zip

:: 1. Build
cd /d %Source%\%3
dotnet test
dotnet build /nodeReuse:false

:: 2. Clear the old publish file
rd /s /q %Publish%\focfoc\%2_publish\%1\%3
mkdir %Publish%\focfoc\%2_publish\%1\%3

:: 3. Copy the new publish file
dotnet publish -c Release -r linux-x64 -o %Publish%\focfoc\%2_publish\%1\%3

:: 4. Create a version
cd /d %Publish%\focfoc\%2_publish\%1\%3
for /f "tokens=1-4 delims=/ " %%i in ('date /t') do set mydate=%%i-%%j-%%k
for /f "tokens=1-2 delims=: " %%i in ('time /t') do set mytime=%%i-%%j
echo Focfoc-%1.%3-1.0.%BUILD_NUMBER% %mydate% %mytime% > version.txt

:: 5. Compress the publish folder
7z a -tzip %ZipFile% .\*
