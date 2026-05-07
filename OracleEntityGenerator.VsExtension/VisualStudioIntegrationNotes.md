# Visual Studio Integration Notes

The installed .NET templates on this machine do not include a VSIX or Visual Studio extensibility template.

This project currently contains the tool window UI and workflow layer as a `net10.0-windows` WPF library. When the Visual Studio 2026 SDK/template is available, add a thin VSIX shell that:

- Registers the Oracle Entity Generator command/menu item.
- Hosts `Views.GeneratorToolWindowControl` inside a Visual Studio tool window.
- Passes selected project/folder context into `GeneratorToolWindowViewModel.OutputDirectory`.
- Keeps all Oracle metadata reading and code generation in the existing non-VS projects.

The business logic remains isolated in:

- `OracleEntityGenerator.Core`
- `OracleEntityGenerator.Oracle`
- `OracleEntityGenerator.CodeGeneration`
