[CmdletBinding()]
param (
    [Parameter(Mandatory=$false)]
    [bool]
    $IsPreview = $true
)

$VersionFile = "$PSScriptRoot/version.txt";

Get-ChildItem Env:

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

$suffix = "~"

if ($IsPreview)
{
    $suffix = "preview."
    $suffix += $Env:GITHUB_RUN_NUMBER
}

"STRAVAIG_PACKAGE_VERSION=$nextVersion" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
"STRAVAIG_PACKAGE_VERSION_SUFFIX=suffix" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
