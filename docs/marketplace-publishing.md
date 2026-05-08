# Publishing to Visual Studio Marketplace

This project can be published to Visual Studio Marketplace using `VsixPublisher.exe`.

Official Microsoft documentation:

https://learn.microsoft.com/en-us/visualstudio/extensibility/walkthrough-publishing-a-visual-studio-extension-via-command-line

## Prerequisites

1. Create or use a Visual Studio Marketplace publisher account.
2. Confirm the publisher ID matches `.marketplace/vsix-publish.json`.
3. Create a Personal Access Token for publishing.
4. Build the VSIX in Release mode.

## Build the VSIX

```powershell
dotnet build OracleEntityGenerator.VSIX/OracleEntityGenerator.VSIX.csproj --configuration Release
```

Output:

```text
OracleEntityGenerator.VSIX/bin/Release/net472/OracleEntityGenerator.VSIX.vsix
```

## Publish Manually

Find `VsixPublisher.exe` under your Visual Studio installation:

```text
%VSINSTALLDIR%\VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe
```

Example command:

```powershell
& "$env:VSINSTALLDIR\VSSDK\VisualStudioIntegration\Tools\Bin\VsixPublisher.exe" publish `
  -payload "OracleEntityGenerator.VSIX/bin/Release/net472/OracleEntityGenerator.VSIX.vsix" `
  -publishManifest ".marketplace/vsix-publish.json" `
  -personalAccessToken "<YOUR_MARKETPLACE_PAT>"
```

Do not commit or log the Personal Access Token.

## Publish from GitHub Actions

Add this repository secret:

```text
VS_MARKETPLACE_PAT
```

Then run the `Publish to Visual Studio Marketplace` workflow manually from GitHub Actions.

## Versioning

The Marketplace version comes from:

```text
OracleEntityGenerator.VSIX/source.extension.vsixmanifest
```

Increase the VSIX manifest version before publishing a new public version.
