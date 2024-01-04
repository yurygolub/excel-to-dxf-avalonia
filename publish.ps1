param(
    [System.String]
    $SingleFile = 'true'
)

if (!(Get-Command -ErrorAction Ignore -Type Application dotnet)) {
    Write-Warning 'dotnet was not found'
    Write-Host 'you have to install .NET SDK 7.0 to build this application'
    Write-Host 'you can download it here https://dotnet.microsoft.com/en-us/download/dotnet/'
    return
}

dotnet publish ExcelToDxfAvalonia --configuration Release --output publish --property:DebugType=None --property:DebugSymbols=false --property:PublishSingleFile=$SingleFile --property:IncludeNativeLibrariesForSelfExtract=true --self-contained --os win
