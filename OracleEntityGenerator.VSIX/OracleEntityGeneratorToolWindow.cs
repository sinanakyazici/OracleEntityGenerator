using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using OracleEntityGenerator.VsExtension.Views;

namespace OracleEntityGenerator.VSIX;

[Guid(Guids.ToolWindowString)]
public sealed class OracleEntityGeneratorToolWindow : ToolWindowPane
{
    public OracleEntityGeneratorToolWindow()
        : base(null)
    {
        Caption = "Oracle Entity Generator";
        Content = new GeneratorToolWindowControl();
    }
}
