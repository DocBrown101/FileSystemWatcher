$BuildPath = "$PSScriptRoot\build"
$Version = Get-Date -Format "yyyy-MM-dd" # 2020-11-1
$VersionDot = $Version -replace '-','.'
$Project = "FileSystemWatcher"
$Archive = "$BuildPath\File-System-Watcher-$Version-x64.zip"

# Clean up
if(Test-Path -Path $BuildPath)
{
    Remove-Item $BuildPath -Recurse
}

# Dotnet restore and build
dotnet publish "$PSScriptRoot\src\$Project\$Project.csproj" `
	   --runtime win-x64 `
	   --self-contained false `
	   -c Release `
	   -v minimal `
	   -o $BuildPath `
	   -p:PublishReadyToRun=true `
	   -p:PublishSingleFile=true `
	   -p:CopyOutputSymbolsToPublishDirectory=false `
	   -p:Version=$VersionDot `
	   --nologo

# Archiv Build
Compress-Archive -Path "$BuildPath\$Project.exe" -DestinationPath $Archive
