using System;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Shell;

namespace OracleEntityGenerator.VSIX;

internal sealed class OpenGeneratorToolWindowCommand
{
    private readonly AsyncPackage package;

    private OpenGeneratorToolWindowCommand(AsyncPackage package, OleMenuCommandService commandService)
    {
        this.package = package;

        var commandId = new CommandID(Guids.CommandSet, CommandIds.OpenGeneratorToolWindow);
        var command = new MenuCommand(Execute, commandId);
        commandService.AddCommand(command);
    }

    public static async System.Threading.Tasks.Task InitializeAsync(AsyncPackage package)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

        var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
        if (commandService is not null)
        {
            _ = new OpenGeneratorToolWindowCommand(package, commandService);
        }
    }

    private void Execute(object sender, EventArgs e)
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        _ = package.JoinableTaskFactory.RunAsync(async () =>
        {
            var window = await package.ShowToolWindowAsync(
                typeof(OracleEntityGeneratorToolWindow),
                id: 0,
                create: true,
                cancellationToken: package.DisposalToken);

            if (window?.Frame is null)
            {
                throw new InvalidOperationException("Oracle Entity Generator tool window could not be created.");
            }
        });
    }
}
