& ./publish.ps1 -SingleFile false

if (!(Get-Command -ErrorAction Ignore -Type Application iscc)) {
    Write-Warning 'iscc was not found'
    return
}

iscc create-installer.iss
