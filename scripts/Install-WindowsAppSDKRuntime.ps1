[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)]
    [ValidateSet($true, $false)]
    [bool]$experimental
)

# Ensure the error action preference is set to the default for PowerShell3, 'Stop'
$ErrorActionPreference = 'Stop'

# Constants
$PackageId = "Microsoft.WindowsAppSDK.Runtime"

# FUNCTIONS #

function Test-Admin {
    $identity = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal $identity
    $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

function Get-LatestNugetVersion {
    Write-Host "Getting the latest package version from https://api.nuget.org`n"

    $registrationUrl = "https://api.nuget.org/v3/registration5-semver1/$($PackageId.ToLower())/index.json"
    $response = Invoke-RestMethod -Uri $registrationUrl -Method Get

    $allEntries = $response.items.items.catalogEntry

    if ($experimental) {
        # Latest including prerelease
        $latestEntry = $allEntries | Select-Object -Last 1
    }
    else {
        # Latest stable (no '-' in version)
        $stableEntries = $allEntries | Where-Object { $_.version -notmatch '-' }
        $latestEntry = $stableEntries | Select-Object -Last 1
    }

    if (-not $latestEntry) {
        throw "Unable to determine latest package entry for $PackageId."
    }

    return [pscustomobject]@{
        Version           = $latestEntry.version
        PackageContentUrl = $latestEntry.packageContent
    }
}

function Get-SystemArchitecture {
    $arch = $env:PROCESSOR_ARCHITECTURE

    $result = switch ($arch.ToLower()) {
        "amd64" { "x64"; break }
        "arm64" { "arm64"; break }
        "x86"   { "x86"; break }
        default { $arch }
    }

    Write-Host "`nDetected system architecture: $result"

    return $result
}

# START OF SCRIPT EXECUTION #

$experimental = if ($experimental) { [bool]::Parse($experimental) } else { $false }

try
{
    if (-not (Test-Admin)) {
        throw "Sorry, you are not an administrator."
    }

    # Query the NuGet API for the latest version
    $latestVersion = Get-LatestNugetVersion
    Write-Host (($latestVersion | Format-List | Out-String).Trim())

    # Detect system architecture
    $architecture = (Get-SystemArchitecture)

    # Download the package to the temporary directory but only if it doesn't already exist
    $packagePath = Join-Path -Path $env:TEMP -ChildPath "WindowsAppSDKRuntime.nupkg.zip"
    if (-not (Test-Path -Path $packagePath)) {
        Write-Host "`nDownloading the package from $($latestVersion.PackageContentUrl)..."
        $packageUrl = $latestVersion.PackageContentUrl
        Invoke-WebRequest -Uri $packageUrl -OutFile $packagePath
    } else {
        Write-Host "`nPackage already downloaded."
    }

    # Extract the package contents if the directory doesn't already exist
    $extractedPath = Join-Path -Path $env:TEMP -ChildPath "WindowsAppSDKRuntime"
    if (-not (Test-Path -Path $extractedPath)) {
        Write-Host "`nExtracting the package to $($extractedPath)..."
        Expand-Archive -Path $packagePath -DestinationPath $extractedPath
    } else {
        Write-Host "`nPackage already extracted."
    }

    # Load the json structure from the 'WindowsAppSDK-VersionInfo.json' file in the extracted path looking
    # at the Runtime node version string
    $versionInfo = Get-Content -Path (Join-Path -Path $extractedPath -ChildPath "WindowsAppSDK-VersionInfo.json") | ConvertFrom-Json
    $version = $versionInfo.Runtime.Version.String
    Write-Host "`nRuntime version: $version"

    # Using the Get-AppxPackage cmdlet, check if the package is already installed. Note the -Name parameter here
    $package = Get-AppxPackage -Name "Microsoft.WindowsAppRuntime*" | Where-Object { $_.Architecture -eq $architecture } | Where-Object { $_.Version -eq $version }
    if ($package) {
        # Exit the script early with a success message, not this is a github action so we need to exit with a success code
        Write-Host "`nThe latest package is already installed. Exiting..."
        Exit 0
    }

    # Install the .msix packages
    # The .msix files will be located in the extracted path under tools/MSIX/win10-$architecture
    
    $msixFiles = Get-ChildItem -Path (Join-Path -Path $extractedPath -ChildPath "tools/MSIX/win10-$architecture") -Filter *.msix
    if (-not $msixFiles) {
        Write-Error "Unable to find the .msix packages."
        Exit 1
    }

    Write-Host ""

    foreach ($msix in $msixFiles) {
        Write-Host "Installing $($msix.FullName)..."
        Add-AppxPackage -Path $msix.FullName -ForceApplicationShutdown -ForceUpdateFromAnyVersion
    }

    Write-Host "`nInstallation complete."

    # Finally verify the latest version is now installed.
    $latestInstalledPackage = Get-AppxPackage -Name "Microsoft.WindowsAppRuntime*" | Where-Object { $_.Architecture -eq $architecture } | Where-Object { $_.Version -eq $version }
    if (-not $latestInstalledPackage) {
        Write-Error "Unable to verify the latest installed version is installed."
        Exit 1
    }
    
    Exit 0
}
catch
{
    Write-Error "Failed to retrieve version: $_"
}