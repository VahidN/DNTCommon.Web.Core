dotnet restore --ignore-failed-sources -v diag
dotnet list package --outdated
rem dotnet restore --ignore-failed-sources -v diag --source https://mirror-nuget.runflare.com/v3/index.json
rem dotnet list package --outdated --source https://mirror-nuget.runflare.com/v3/index.json
dotnet restore --ignore-failed-sources -v diag
pause