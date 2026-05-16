# Visual Studio Integration Notes

The installed .NET templates on this machine do not include a VSIX or Visual Studio extensibility template.

The VSIX shell hosts the tool window UI while keeping generation logic in testable libraries. The shell should:

- Registers the Oracle Entity Generator command/menu item.
- Hosts `Views.GeneratorToolWindowControl` inside a Visual Studio tool window.
- Passes selected project/folder context into the entity and configuration folder selectors when project detection is available.
- Keeps all Oracle metadata reading and code generation in the existing non-VS projects.

The business logic remains isolated in:

- `OracleEntityGenerator.Core`
- `OracleEntityGenerator.Oracle`
- `OracleEntityGenerator.CodeGeneration`
