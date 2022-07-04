# Local Installation and Update

## Local Installation

To install this package locally directly, add the location of the package to the ReSharper Extension Manager (In Visual Studio go to Extensions | Resharper | Options... | Environment | Extension Manager | Add). 

You should now see it as an available extension. 

## Update

You can update this extension to the newest version of ReSharper or any other version.

1. Clone the repository.
2. Update the NuGet package `JetBrains.ReSharper.SDK` to the version of ReSharper you want to target.
3. Build the project.
4. Update the file `ExceptionalDevs.Exceptional.nuspec` in the folder `build` with the targeted version.
5. Run `CreatePackage.bat` in the folder `build`, a new package should be created under `Packages`.
6. You should now be able to install it locally.

If it does not show up:
1. Try to install this package as a local NuGet package.
2. It might fail with a version conflict for Wave. In this case, update the file `ExceptionalDevs.Exceptional.nuspec` in the folder `build` with the version required by ReSharper.
3. Run `CreatePackage.bat` in the folder `build`, a new package should be created under `Packages`.
4. You should now be able to install it locally.

## Development Build

You can find a full guide on [JetBrains.com](https://www.jetbrains.com/help/resharper/sdk/HowTo/Start/SetUpEnvironment.html).

## Debugging

Please read this [guide](https://www.jetbrains.com/help/resharper/sdk/Extensions/Plugins/Debugging.html).

## Troubleshooting

### I cannot install my local version

Uninstall previous installations of Exceptional for ReSharper and clear `C:\Users\%username%\AppData\Local\JetBrains\plugins` and `C:\Users\%username%\AppData\Local\NuGet\Cache`, after that restart VS.
