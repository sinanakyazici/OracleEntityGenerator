# Oracle Entity Generator

Generate C# entity classes and EF Core configuration classes from Oracle Database table metadata directly inside Visual Studio.

## What It Does

Oracle Entity Generator connects to an Oracle Database, discovers schemas and tables available to the connected user, and generates:

- Clean C# entity classes
- EF Core `IEntityTypeConfiguration<T>` classes
- Table and column mappings that preserve original Oracle names
- Nullable reference type aware output
- Optional XML comments from Oracle metadata

## Key Features

- Connect using host, port, service name, username, and password
- Supports full Oracle connection string input
- Reads accessible metadata through Oracle `ALL_*` views
- Lists schemas and tables available to the current user
- Generates entity and configuration files into selected folders
- Supports overwrite control
- Supports configurable namespaces
- Supports configurable table and column prefix removal
- Does not singularize or pluralize Oracle object names

## Naming Policy

Oracle object names are authoritative.

- `CUSTOMERS` generates `Customers`, not `Customer`
- `ORDER_LINES` generates `OrderLines`, not `OrderLine`
- `CUSTOMER_ID` generates `CustomerId`

The extension only converts Oracle names into valid C# identifiers. It does not infer domain names or apply English grammar rules.

## Security

- Credentials are entered by the user
- Passwords are not hardcoded
- Passwords are not written to generated files
- Logs should not contain secrets

## Source Code

Source code, documentation, and issue tracking are available on GitHub:

https://github.com/sinanakyazici/OracleEntityGenerator
