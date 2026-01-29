# Release Notes

## Version X

Date: ???

# Bug fixes
- Category names for types has been made consistent. There were inconsistencies between the category name for inner types (e.g. a class within a class) depending on how the logger was created.

# Breaking changes
- If you are checking the category name for an inner type, in some cases it will now return a slight differently formatted name as a result of a bug fix. e.g. `OuterType+InnerType` will now return `OuterType.InnerType`.
- Support for .NET 6.0 was dropped.

### Miscellaneous

- Drop support for .NET 6.0
- Add support for .NET 10.0
- Various code optimisations and clean up.


### Dependencies

- .NET 9.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 9.0.12
- .NET 10.0 targets:
  - Initial support
