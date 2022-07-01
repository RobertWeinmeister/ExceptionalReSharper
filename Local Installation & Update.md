# Local Installation and Update

## Local Installation

To install this package locally directly, add the location of the package to the ReSharper Extension Manager (In Visual Studio go to Extensions | Resharper | Options... | Environment | Extension Manager | Add). 
2. 


    - In VS => Extensions => Resharper => Extension Manager => "Find the extension & Install it"

2. New Build
    - Change directory to the build directory `cd path\to\ExceptionalReSharper\build`
    - Run `build.ps1`
    - Now you should find a new package in `path\to\ExceptionalReSharper\build\Packages`
    - Copy the *.nupkg file to your local repository
    - In VS => Extensions => Resharper => Options => Extension Manager => Add => 
        Name: Exceptional Local
        Path: `path\to\ExceptionalReSharper\build\Packages` (Recommended to clone the Repo on the local volume "C:")
    - In VS => Extensions => Resharper => Extension Manager => "Find the extension & Install it"   

## Update

You can update this extension to the newest version of ReSharper or any other version.

1. Clone the repository.
2. Update the NuGet package `JetBrains.ReSharper.SDK` to the version of ReSharper you want to target.
3. Update the file `ExceptionalDevs.Exceptional.nuspec` in the folder `build` with the targeted version.
4. Build the project.
5. Run `CreatePackage.bat` in the folder `build`, a new package should be created under `Packages`.

## Development Build

You can find a full guide on [JetBrains.com](https://www.jetbrains.com/help/resharper/sdk/HowTo/Start/SetUpEnvironment.html).

## Debugging

Please read this [guide](https://www.jetbrains.com/help/resharper/sdk/Extensions/Plugins/Debugging.html).

## Troubleshooting

### I cannot install my local version

Uninstall previous installations of Exceptional for ReSharper and clear `C:\Users\%username%\AppData\Local\JetBrains\plugins` and `C:\Users\%username%\AppData\Local\NuGet\Cache`, after that restart VS.