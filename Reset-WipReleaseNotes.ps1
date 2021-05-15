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
"- .NET Standard 2.0 targets:"
"- .NET 5.0 targets:"
"",
"",
""
)

Set-Content "$PSScriptRoot/release-notes/wip-release-notes.md" $content -Encoding UTF8 -Force