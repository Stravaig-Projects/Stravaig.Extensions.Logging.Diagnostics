$content = @(
"# Release Notes",
"",
"## Version X",
"",
"Date: ???",
""
"### Bugs"
"",
"### Features",
"",
"### Miscellaneous",
"",
"### Dependencies",
"",
"- All targets:"
"- .NET 6.0 targets:"
"- .NET 7.0 targets:"
"- .NET 8.0 targets:"
"",
"",
""
)

Set-Content "$PSScriptRoot/release-notes/wip-release-notes.md" $content -Encoding UTF8 -Force