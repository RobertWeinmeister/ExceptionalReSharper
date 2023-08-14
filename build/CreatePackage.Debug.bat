copy "..\src\Exceptional\bin\Debug\ReSharper.Exceptional.dll" "lib\ReSharper.Exceptional.dll"
copy "..\src\Exceptional\bin\Debug\ReSharper.Exceptional.pdb" "lib\ReSharper.Exceptional.pdb"
nuget pack ExceptionalDevs.Exceptional.Debug.nuspec -Verbosity detailed -OutputDirectory ./Packages
pause