using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.VisualStudio.Shell;

namespace OracleEntityGenerator.VSIX;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration("Oracle Entity Generator", "Generates EF Core entity and configuration classes from Oracle Database metadata.", "1.0")]
[ProvideMenuResource("Menus.ctmenu", 1)]
[ProvideToolWindow(typeof(OracleEntityGeneratorToolWindow))]
[Guid(Guids.PackageString)]
public sealed class OracleEntityGeneratorPackage : AsyncPackage
{
    protected override async System.Threading.Tasks.Task InitializeAsync(
        CancellationToken cancellationToken,
        IProgress<ServiceProgressData> progress)
    {
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        await OpenGeneratorToolWindowCommand.InitializeAsync(this);
    }
}
