$Version = "2020-11-1"
$VersionDot = $Version -replace '-','.'
$BuildPath = "$PSScriptRoot\build"

# Clean up
if(Test-Path -Path $BuildPath)
{
    Remove-Item $BuildPath -Recurse
}

# Dotnet restore and build
dotnet publish "$PSScriptRoot\src\FileSystemWatcher\FileSystemWatcher.csproj" `
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
Compress-Archive -Path "$BuildPath\FileSystemWatcher.exe" -DestinationPath "$BuildPath\$Version.zip"
