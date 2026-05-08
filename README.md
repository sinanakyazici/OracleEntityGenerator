# Oracle Entity Generator

[![CI](https://github.com/sinanakyazici/OracleEntityGenerator/actions/workflows/ci.yml/badge.svg)](https://github.com/sinanakyazici/OracleEntityGenerator/actions/workflows/ci.yml)
[![Release](https://github.com/sinanakyazici/OracleEntityGenerator/actions/workflows/release.yml/badge.svg)](https://github.com/sinanakyazici/OracleEntityGenerator/actions/workflows/release.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

Oracle Entity Generator is a Visual Studio extension that generates clean C# entity classes and EF Core `IEntityTypeConfiguration<T>` classes from Oracle Database table metadata.

The project is designed as a reusable generator, not just a Visual Studio UI. The core metadata, naming, type mapping, and code generation logic is isolated from the VSIX layer so it can be tested and evolved independently.

## Highlights

- Connect to Oracle Database using host, port, service name, username, and password.
- Supports full Oracle connection strings, including Easy Connect and TNS descriptor formats.
- Lists accessible schemas and tables through Oracle `ALL_*` metadata views.
- Generates persistence-ignorant C# entity classes.
- Generates separate EF Core configuration classes.
- Preserves original Oracle table and column names in EF Core mappings.
- Does not singularize or pluralize table or column names.
- Supports nullable reference type output.
- Supports XML comments from Oracle comments.
- Supports configurable table and column prefix removal.
- Includes a Visual Studio tool window and a standalone playground host.
- Includes automated CI, tests, VSIX packaging, and GitHub release publishing.

## Installation

Download the latest VSIX from the [GitHub Releases](https://github.com/sinanakyazici/OracleEntityGenerator/releases) page and install it by opening the `.vsix` file.

After installation, open Visual Studio and use:

```text
Tools -> Oracle Entity Generator
```

The extension is prepared for Visual Studio Marketplace publishing. Marketplace publishing instructions are available in [docs/marketplace-publishing.md](docs/marketplace-publishing.md).

## Requirements

- Visual Studio 2026 / Visual Studio 18 Insiders, or a compatible Visual Studio version with VSIX support.
- Visual Studio extension development workload for local development.
- Oracle Database access with read privileges on the target schemas and tables.
- .NET SDK 10 preview for building the full solution.

The VSIX package targets `net472` for Visual Studio compatibility. The generator libraries multi-target `net472` and `net10.0`.

## Supported Oracle Discovery

The extension uses metadata views available to regular users:

- `ALL_TABLES`
- `ALL_TAB_COLUMNS`
- `ALL_CONSTRAINTS`
- `ALL_CONS_COLUMNS`
- `ALL_COL_COMMENTS`
- `ALL_TAB_COMMENTS`

It intentionally avoids `DBA_*` views because users may not have DBA privileges.

## Naming Rules

Oracle object names are the source of truth.

- `CUSTOMERS` becomes `Customers`, not `Customer`.
- `CUSTOMER_ORDER` becomes `CustomerOrder`.
- `ORDER_LINES` becomes `OrderLines`, not `OrderLine`.
- `CUSTOMER_ID` becomes `CustomerId`.
- `IS_ACTIVE` becomes `IsActive`.

The generator does not apply English grammar rules, domain-name guessing, singularization, or pluralization. Only the minimum C# identifier transformation is applied, plus optional prefix removal when explicitly configured.

## Generated Output

Entity classes are clean POCOs:

```csharp
namespace MyProject.Domain.Entities;

public class CustomerOrder
{
    public int CustomerId { get; set; }

    public string OrderNumber { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
```

Configuration classes preserve Oracle names:

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyProject.Domain.Entities;

namespace MyProject.Infrastructure.Persistence.Configurations;

public sealed class CustomerOrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
{
    public void Configure(EntityTypeBuilder<CustomerOrder> builder)
    {
        builder.ToTable("CUSTOMER_ORDER", "CRM");

        builder.HasKey(x => x.CustomerId);

        builder.Property(x => x.CustomerId)
            .HasColumnName("CUSTOMER_ID")
            .IsRequired();

        builder.Property(x => x.OrderNumber)
            .HasColumnName("ORDER_NUMBER")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();
    }
}
```

## Solution Structure

```text
OracleEntityGenerator.Core
  Metadata models, naming services, type mapping, and generator contracts.

OracleEntityGenerator.Oracle
  Oracle connection handling and metadata reader implementation.

OracleEntityGenerator.CodeGeneration
  Entity and EntityTypeConfiguration code generation.

OracleEntityGenerator.VsExtension
  WPF tool window UI used by the Visual Studio extension.

OracleEntityGenerator.VSIX
  Visual Studio package, command registration, and VSIX packaging.

OracleEntityGenerator.VsExtension.Playground
  Standalone WPF host for local UI development and debugging.

OracleEntityGenerator.Tests
  Unit tests, smoke tests, and generated output validation.
```

## Build

```powershell
dotnet restore OracleEntityGenerator.slnx
dotnet build OracleEntityGenerator.slnx --configuration Release
```

To run tests:

```powershell
dotnet test OracleEntityGenerator.Tests/OracleEntityGenerator.Tests.csproj --configuration Release
```

The VSIX is produced under:

```text
OracleEntityGenerator.VSIX/bin/Release/net472/OracleEntityGenerator.VSIX.vsix
```

## Local UI Testing

For quick UI validation without installing the extension, run the playground project from Visual Studio:

```text
OracleEntityGenerator.VsExtension.Playground
```

For real Visual Studio integration testing, build the VSIX project and install the generated `.vsix`.

## Security

- Passwords are never hardcoded.
- Passwords are not written to generated files.
- Logs must not contain credentials.
- Connection profiles should not persist passwords unless secure storage is implemented.

Please report security issues using the guidance in [SECURITY.md](SECURITY.md).

## Releases

CI builds, tests, and packages the VSIX on every push to `main`.

After CI succeeds, the release workflow downloads the VSIX artifact produced by CI, reads the VSIX manifest version, and creates or updates a GitHub release such as:

```text
Oracle Entity Generator v1.0.6
```

Release notes are generated by GitHub.

## Contributing

Contributions are welcome. Please read [CONTRIBUTING.md](CONTRIBUTING.md) before opening a pull request.

## License

This project is licensed under the [MIT License](LICENSE).
