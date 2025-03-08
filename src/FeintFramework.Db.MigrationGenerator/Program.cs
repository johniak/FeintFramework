
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FeintFramework.Core.Config.Settings;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using FeintFramework.Core.Apps;

namespace FeintFramework.Db.MigrationGenerator;
class Program
{
    static async Task<int> Main(string[] args)
    {
        var projectOption = new Option<string>(new[] { "--project", "-p" }, "The project directory path.");
        var appOption = new Option<string>(new[] { "--app", "-a" }, "The app name.");
        var settingsOption = new Option<string>(new[] { "--settings", "-s" }, "The full settings class name.");

        // Create the root command and add options
        var rootCommand = new RootCommand("This command will generate migration files for the given project, or app.")
            {
                projectOption,
                appOption,
                settingsOption
            };
        rootCommand.SetHandler((string project, string app, string settings) =>
        {
            var exitCode = RunMigration(project, app, settings);
            return Task.FromResult(exitCode);
        }, projectOption, appOption, settingsOption);

        rootCommand.AddValidator(result =>
        {
            var appValue = result.GetValueForOption(appOption);
            var settingsValue = result.GetValueForOption(settingsOption);
            if (string.IsNullOrEmpty(appValue) && string.IsNullOrEmpty(settingsValue))
            {
                result.ErrorMessage = "Either --app or --settings must be provided.";
            }
        });

        return await rootCommand.InvokeAsync(args);
    }
    static int RunMigration(string projectPath = ".", string? appName = null, string? settingsClassFullName = null)
    {
        // args = ["", "Example.Core.Settings"];
        // if (args.Length < 2)
        // {
        //     Console.WriteLine("Usage: MigrationTool <projectFolder> <fullSettingsClassName>");
        //     return 1;
        // }

        string projectDirectory = Path.GetFullPath(projectPath);

        if (!Directory.Exists(projectDirectory))
        {
            Console.WriteLine($"Project folder does not exist: {projectDirectory}");
            return 1;
        }
        var csprojFiles = Directory.GetFiles(projectDirectory, "*.csproj", SearchOption.TopDirectoryOnly);
        if (csprojFiles.Length == 0)
        {
            Console.WriteLine("No .csproj file found in the project folder.");
            return 1;
        }
        string csprojFile = csprojFiles[0];
        Console.WriteLine("Found project file: " + csprojFile);
        ProcessStartInfo psi = new ProcessStartInfo("dotnet", $"build \"{csprojFile}\"")
        {
            WorkingDirectory = projectDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        using (Process process = Process.Start(psi))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                Console.WriteLine("Build failed:");
                Console.WriteLine(output);
                Console.WriteLine(error);
                return 1;
            }
            Console.WriteLine("Build succeeded:");
            Console.WriteLine(output);
        }

        string projectName = Path.GetFileNameWithoutExtension(csprojFile);
        string binDebugFolder = Path.Combine(projectDirectory, "bin", "Debug");
        if (!Directory.Exists(binDebugFolder))
        {
            Console.WriteLine("No bin/Debug folder found.");
            return 1;
        }
        string[] assemblyFiles = Directory.GetFiles(binDebugFolder, projectName + ".dll", SearchOption.AllDirectories);
        string[] pdbFiles = Directory.GetFiles(binDebugFolder, projectName + ".pdb", SearchOption.AllDirectories);
        if (assemblyFiles.Length == 0)
        {
            Console.WriteLine($"No assembly file '{projectName}.dll' found in bin/Debug.");
            return 1;
        }
        string assemblyPath = assemblyFiles[0];
        string pdbPath = pdbFiles[0];
        Console.WriteLine("Loading assembly: " + assemblyPath);
        Assembly asm = Assembly.LoadFrom(assemblyPath);
        Type[] applicationTypes = [];
        if (settingsClassFullName != null && appName == null)
        {


            Type settingsType = asm.GetType(settingsClassFullName);

            if (settingsType == null)
            {
                Console.WriteLine($"Type '{settingsClassFullName}' not found in assembly.");
                return 1;
            }

            var settingsInstance = (BaseSettings)Activator.CreateInstance(settingsType);
            if (settingsInstance == null)
            {
                Console.WriteLine("Failed to create an instance of type: " + settingsType.FullName);
                return 1;
            }
            applicationTypes = settingsInstance.InstalledApps;
        }
        else if (appName != null)
        {
            applicationTypes = [getApplicationType(appName, asm)!];
        }
        if (applicationTypes.Length == 0)
        {
            Console.WriteLine("You have to pass valid app name or settings class full name");
            return 1;
        }
        var migrationGenerator = new MigrationGenerator(applicationTypes, projectDirectory);
        migrationGenerator.GenerateMigration();
        return 0;
    }

    static Type? getApplicationType(string appName, Assembly assembly)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            if (!type.IsAssignableTo(typeof(BaseApplication)))
                continue;
            var applicationInstance = (BaseApplication)Activator.CreateInstance(type)!;
            if (applicationInstance.Name == appName)
                return type;
        }
        return null;
    }
}

