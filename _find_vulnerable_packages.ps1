dotnet list package --vulnerable --include-transitive
dotnet nuget why DNTCommon.Web.Core.slnx System.Security.Cryptography.Pkcs
rem dotnet list package --include-transitive
rem dotnet list package --include-transitive | Select-String "System.Security.Cryptography.Pkcs"
pause