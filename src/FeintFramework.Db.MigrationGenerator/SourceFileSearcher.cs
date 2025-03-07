using System.Text.RegularExpressions;
namespace FeintFramework.Db.MigrationGenerator;
public class SourceFileSearcher
{
    public static string FindSourceFileForType(string projectDirectory, string fullTypeName)
    {
        int lastDotIndex = fullTypeName.LastIndexOf('.');
        if (lastDotIndex < 0)
        {
            Console.WriteLine("Full type name must include a namespace.");
            return null;
        }
        string namespaceName = fullTypeName.Substring(0, lastDotIndex);
        string className = fullTypeName.Substring(lastDotIndex + 1);

        string namespacePattern = $@"\bnamespace\s+{Regex.Escape(namespaceName)}\b";
        string classPattern = $@"\bclass\s+{Regex.Escape(className)}\b";
        string[] files = Directory.GetFiles(projectDirectory, "*.cs", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                string content = File.ReadAllText(file);
                if (Regex.IsMatch(content, namespacePattern) && Regex.IsMatch(content, classPattern))
                {
                    return file;
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading file '{file}': {ex.Message}");
            }
        }
        return null;
    }
}