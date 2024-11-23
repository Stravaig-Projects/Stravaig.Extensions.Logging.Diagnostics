$VersionFile = "$PSScriptRoot/version.txt";
$outputFolder = "./out";
$versionEnvFile = "$outputFolder/version-info.env";

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

if (-not (Test-Path $outputFolder))
{
    $null = New-Item $outputFolder -Type Directory;
}

$suffix = "preview."
$suffix += $Env:GITHUB_RUN_NUMBER

$previewVersion = "$nextVersion-$suffix";
$envContent = "STRAVAIG_PACKAGE_VERSION=$nextVersion" + [System.Environment]::NewLine +
"STRAVAIG_PACKAGE_VERSION_SUFFIX=$suffix" + [System.Environment]::NewLine +
"STRAVAIG_STABLE_VERSION=$nextVersion" + [System.Environment]::NewLine +
"STRAVAIG_PREVIEW_VERSION=$previewVersion";

Write-Host $envContent;

$envContent | Out-File -FilePath $versionEnvFile -Encoding UTF8
$envContent | Out-File -FilePath $env:GITHUB_ENV -Encoding UTF8 -Append
