set NuGetAudit=false

dotnet restore --ignore-failed-sources -v diag --source %USERPROFILE%\.nuget\packages
pause

dotnet restore --ignore-failed-sources -v diag --source https://nuget.devneeds.ir/repository/nuget/index.json
dotnet restore --ignore-failed-sources -v diag --source https://mirror2.chabokan.net/nuget/v3/index.json
dotnet restore --ignore-failed-sources -v diag --source https://mirror.abrha.net/repository/nuget/index.json
dotnet restore --ignore-failed-sources -v diag --source https://mirror-nuget.runflare.com/v3/index.json
dotnet restore --ignore-failed-sources -v diag --source https://package-mirror.liara.ir/repository/nuget/index.json
dotnet restore --ignore-failed-sources -v diag --source https://api.nuget.org/v3/index.json
pause