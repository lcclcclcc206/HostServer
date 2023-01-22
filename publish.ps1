# publish the project
if ((Test-Path .\publish) -eq $True) {
    Get-ChildItem publish | Remove-Item -Recurse
}
dotnet publish -o publish