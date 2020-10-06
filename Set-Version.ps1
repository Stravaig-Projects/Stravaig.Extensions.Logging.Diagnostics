Get-ChildItem Env:

"STRAVAIG_PACKAGE_VERSION=1.2.3" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
"STRAVAIG_PACKAGE_VERSION_SUFFIX=some-suffix" | Out-File -FilePath $Env:GITHUB_ENV -Encoding UTF8 -Append
