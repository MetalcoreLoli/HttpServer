var target = Argument("target", "Publish");
var configuration = Argument("configuration", "Release");
var solutionFolder = "./";
var outputFolder= "./artifacts";

// TASKS

Task("Clean")
    .Does(() => 
    {
        CleanDirectory(outputFolder);
        CleanDirectory($"./src/HttpServer/bin/{configuration}");
    });

Task("Restore").Does(() => {
    DotNetRestore(solutionFolder);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        DotNetBuild(solutionFolder, new DotNetBuildSettings 
        {
            Configuration = configuration,
            NoRestore = true
        });
    });

Task("Test")
    .IsDependentOn("Build")
    .Does(() => 
    {
        DotNetTest(solutionFolder, new DotNetTestSettings 
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
        });
    });

Task("Publish")
//    .IsDependentOn("Test")
    .IsDependentOn("Build")
    .Does (() => 
    {
        DotNetPublish(solutionFolder, new DotNetPublishSettings 
        {
            NoRestore = true,
            NoBuild = true,
            Configuration = configuration,
            OutputDirectory = outputFolder
        });
    });

// RUN

RunTarget(target);
