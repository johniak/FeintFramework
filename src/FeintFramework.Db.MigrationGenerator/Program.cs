
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using FeintFramework.Core.Config.Settings;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

namespace FeintFramework.Db.MigrationGenerator;
class Program
{
    static int Main(string[] args)
    {
        args = ["../../../../../Example", "Example.Core.Settings"];
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: MigrationTool <projectFolder> <fullSettingsClassName>");
            return 1;
        }

        string projectFolder = Path.GetFullPath(args[0]);
        string settingsClassFullName = args[1];

        if (!Directory.Exists(projectFolder))
        {
            Console.WriteLine($"Project folder does not exist: {projectFolder}");
            return 1;
        }
        var csprojFiles = Directory.GetFiles(projectFolder, "*.csproj", SearchOption.TopDirectoryOnly);
        if (csprojFiles.Length == 0)
        {
            Console.WriteLine("No .csproj file found in the project folder.");
            return 1;
        }
        string csprojFile = csprojFiles[0];
        Console.WriteLine("Found project file: " + csprojFile);
        ProcessStartInfo psi = new ProcessStartInfo("dotnet", $"build \"{csprojFile}\"")
        {
            WorkingDirectory = projectFolder,
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
        string binDebugFolder = Path.Combine(projectFolder, "bin", "Debug");
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

        Console.WriteLine($"Instance of '{settingsType.FullName}' created successfully.");
        Console.WriteLine("Instance: " + settingsInstance.ToString());
        var migrationGenerator = new MigrationGenerator(settingsInstance, projectFolder);
        migrationGenerator.GenerateMigration();
        return 0;
    }
}

