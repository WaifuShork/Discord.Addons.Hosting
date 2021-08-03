function Build
{
    [string]$projectPath = "src\Discord.Addons.Hosting\Discord.Addons.Hosting.csproj";
    [string]$outputDir = "artifacts\"
    dotnet build $projectPath `
                 -o $outputDir `
                 -p:PublishSingleFile=true `
                 -p:PublishTrimmed=true `
                 -p:IncludeNativeLibrariesForSelfExtract=true `
                 --configuration Release;
}

Build;