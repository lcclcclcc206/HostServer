# Deploy executable file to the destination path
# Deploy to IIS

./publish.ps1

$publishPath = "./publish";
# Use absolute path to customize
$deployPath = "D:/HostServer";

Copy-Item -Path "./app_offline.htm" -Destination $deployPath -Force

Set-Location $publishPath

$files = Get-ChildItem -Path ".";

foreach ($item in $files) {
    Copy-Item -Path $item.Name -Destination $deployPath -Recurse -Force
}

Set-Location ".."

Remove-Item -Path "$deployPath/app_offline.htm" -Force

Write-Host "Deploy Complete!"