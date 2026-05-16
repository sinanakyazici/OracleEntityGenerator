# Changelog

All notable changes to this project will be documented in this file.

The project follows version numbers from the VSIX manifest.

## 1.0.10

- Added an optional `HasColumnType(...)` configuration mapping mode.
- Added compact generated code output with inline fluent configuration calls and reduced blank lines.
- Modernized the Visual Studio tool window layout with clearer status, connection, table, generation, and log sections.
- Added folder pickers for entity and configuration directories and removed unused output, using, and prefix fields from the tool window.

## 1.0.9

- Changed Oracle `NUMBER` mapping so columns without scale metadata generate `long`.
- Added precision and scale configuration for Oracle `NUMBER` columns when scale metadata is available.

## 1.0.8

- Replaced the Marketplace icon with a true transparent PNG.

## 1.0.7

- Added a custom Marketplace and VSIX icon.

## 1.0.6

- Added Visual Studio tool window integration.
- Added Oracle connection testing, schema loading, and table loading.
- Added entity and EF Core configuration generation from selected table metadata.
- Added nullable reference type support.
- Added configurable prefixes, headers, using directives, and overwrite behavior.
- Added standalone playground application for UI testing.
- Added CI, test execution, VSIX artifact packaging, and automatic GitHub release publishing.
- Improved Visual Studio dark theme support.
