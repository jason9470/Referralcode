param (
    [Parameter(Mandatory=$true)]
    [string]$Url,
    [Parameter(Mandatory=$true)]
    [string]$DestPath
)

try {
    Write-Host "Downloading $Url to $DestPath..."
    Invoke-RestMethod -Uri $Url | Out-File -FilePath $DestPath -Encoding utf8
    Write-Host "Successfully updated $DestPath"
} catch {
    Write-Error "Failed to download $Url. Error: $_"
    exit 1
}
