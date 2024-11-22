[CmdletBinding()]
param (
    [Parameter(Mandatory=$false)]
    [string]
    $IsPreview = $true,

    [Parameter(Mandatory=$false)]
    [string]
    $IsPublicRelease = "false"
)

function ConvertTo-Boolean([string]$Value, [bool]$EmptyDefault)
{
    if ([string]::IsNullOrWhiteSpace($value)) { return $EmptyDefault; }
    if ($Value -like "true") { return $true; }
    if ($Value -like "false") { return $false; }
    throw "Don't know how to convert `"$Value`" into a Boolean."
}

[bool]$IsPreview = ConvertTo-Boolean -Value $IsPreview -EmptyDefault $true;
[bool]$IsPublicRelease = ConvertTo-Boolean -Value $IsPublicRelease -EmptyDefault $false;

$VersionFile = "$PSScriptRoot/version.txt";

# Work out the version number
$nextVersion = Get-Content $VersionFile -ErrorAction Stop
if ($null -eq $nextVersion)
{
    Write-Error "The $VersionFile file is empty"
    Exit 1
}
if ($nextVersion.GetType().BaseType.Name -eq "Array")
{
    $nextVersion = $nextVersion[0]
    Write-Warning "$VersionFile contains more than one line of text. Using the first line."
}
if ($nextVersion -notmatch "^[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}$")
{
    Write-Error "The contents of $VersionFile (`"$nextVersion`") not recognised as a valid version number."
    Exit 2
}

$outputFolder = "./out";
$versionEnvFile = "$ouputFolder/version-info.env";
if (-not (Test-Path $outputFolder))
{
    New-Item $outputFolder -Type Directory;
}


"STRAVAIG_PACKAGE_VERSION=$nextVersion" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
"STRAVAIG_PACKAGE_VERSION=$nextVersion" | Out-File -FilePath $versionEnvFile -Encoding UTF8 -Append

$suffix = "preview."
$suffix += $Env:GITHUB_RUN_NUMBER

"STRAVAIG_PACKAGE_VERSION_SUFFIX=$suffix" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
"STRAVAIG_PACKAGE_VERSION_SUFFIX=$suffix" | Out-File -FilePath $versionEnvFile -Encoding UTF8 -Append

$previewVersion = "$nextVersion-$suffix";
"STRAVAIG_STABLE_PACKAGE_VERSION=$nextVersion" | Out-File -FilePath $versionEnvFile -Encoding UTF8 -Append
"STRAVAIG_PREVIEW_PACKAGE_VERSION=$previewVersion" | Out-File -FilePath $versionEnvFile -Encoding UTF8 -Append
