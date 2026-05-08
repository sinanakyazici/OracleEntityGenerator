## Summary

Describe the change and why it is needed.

## Validation

- [ ] `dotnet build OracleEntityGenerator.slnx --configuration Release`
- [ ] `dotnet test OracleEntityGenerator.Tests/OracleEntityGenerator.Tests.csproj --configuration Release`

## Checklist

- [ ] Oracle names are preserved in EF Core mappings.
- [ ] No singularization or pluralization was introduced.
- [ ] No credentials or secrets are logged or stored.
- [ ] Tests were added or updated where behavior changed.
