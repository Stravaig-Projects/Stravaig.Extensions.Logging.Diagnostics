$content = @(
"# Release Notes",
"",
"## Version X",
"",
"Date: ???",
""
"### Bug Fixes"
"",
"### Features",
"",
"### Miscellaneous",
"",
"### Dependencies",
"",
"- All targets:"
"- .NET 6.0 targets:"
"- .NET 8.0 targets:"
"- .NET 9.0 targets:"
"",
"",
""
)

Set-Content "$PSScriptRoot/release-notes/wip-release-notes.md" $content -Encoding UTF8 -Force
