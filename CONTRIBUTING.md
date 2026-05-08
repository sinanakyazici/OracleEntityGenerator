# Contributing

Thanks for your interest in improving Oracle Entity Generator.

## Development Setup

Install the following:

- Visual Studio 2026 / Visual Studio 18 Insiders or a compatible Visual Studio version.
- Visual Studio extension development workload.
- .NET SDK 10 preview.

Restore and build:

```powershell
dotnet restore OracleEntityGenerator.slnx
dotnet build OracleEntityGenerator.slnx --configuration Release
```

Run tests:

```powershell
dotnet test OracleEntityGenerator.Tests/OracleEntityGenerator.Tests.csproj --configuration Release
```

## Pull Request Guidelines

- Keep changes focused.
- Add or update tests for generator, naming, type mapping, or metadata behavior.
- Do not introduce singularization or pluralization behavior.
- Do not log passwords, connection strings with passwords, or other secrets.
- Preserve Oracle object names in EF Core mappings.
- Keep Visual Studio-specific code inside the VSIX or extension UI projects.
- Keep core generator behavior testable without Visual Studio.

## Naming Policy

Oracle names are authoritative. The generator may convert Oracle names to valid C# identifiers, but it must not infer business names or apply English grammar rules.

Examples:

- `CUSTOMERS` -> `Customers`
- `ORDER_LINES` -> `OrderLines`
- `CUSTOMER_ID` -> `CustomerId`

## Commit Style

Use concise, imperative commit messages, for example:

```text
Add Oracle table metadata reader tests
Fix filtered table selection in tool window
```
