# Oracle Entity Generator

Visual Studio extension for generating C# entity classes and EF Core `IEntityTypeConfiguration<T>` classes from Oracle Database table metadata.

## Current Capabilities

- Connects to Oracle Database with host, port, service name, username, and password.
- Supports full Oracle connection string input, including Easy Connect and TNS descriptor formats.
- Lists schemas and tables available through `ALL_*` metadata views.
- Generates clean entity classes and separate EF Core configuration classes.
- Preserves Oracle object names in EF Core mappings.
- Does not singularize or pluralize table or column names.
- Supports nullable reference type output, XML comments, overwrite control, and configurable prefix removal.
- Provides a Visual Studio tool window through a VSIX extension.

## Projects

- `OracleEntityGenerator.Core`: metadata models, naming, type mapping, generation contracts.
- `OracleEntityGenerator.Oracle`: Oracle connection and metadata reader.
- `OracleEntityGenerator.CodeGeneration`: entity and configuration code generation.
- `OracleEntityGenerator.VsExtension`: WPF tool window UI.
- `OracleEntityGenerator.VSIX`: Visual Studio extension package.
- `OracleEntityGenerator.VsExtension.Playground`: standalone UI host for local testing.
- `OracleEntityGenerator.Tests`: unit and smoke tests.

## Notes

- Core libraries multi-target `net472` and `net10.0`.
- The Visual Studio extension runs on `net472` for VSIX compatibility.
- Credentials are entered by the user and are not hardcoded.
- Local generated output and build artifacts are excluded from source control.
