using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;

namespace FeintFramework.Config;

public class HtmlWatcher
{
    private readonly List<string> projectDirs;
    private readonly string outputRoot;
    private readonly HashSet<string> extensions;
    private readonly List<FileSystemWatcher> watchers = new();
    private readonly Dictionary<string, Timer> debounceTimers = new(StringComparer.OrdinalIgnoreCase);
    private readonly TimeSpan debounceInterval = TimeSpan.FromMilliseconds(500);

    public HtmlWatcher(IEnumerable<string> extensions)
    {
        outputRoot = Directory.GetCurrentDirectory();
        var rootCsproj = FindCsprojUpwards(outputRoot);
        if (rootCsproj == null)
            throw new FileNotFoundException("No .csproj found.");

        projectDirs = GetProjectDirectories(rootCsproj);
        this.extensions = new HashSet<string>(
            extensions.Select(e => e.StartsWith('.') ? e.ToLower() : "." + e.ToLower()),
            StringComparer.OrdinalIgnoreCase
        );
    }

    public void Start()
    {
        var thread = new Thread(Run)
        {
            IsBackground = true,
            Name = "HtmlWatcherThread"
        };
        thread.Start();
    }

    private void Run()
    {
        Console.WriteLine("Watching extensions: {0}", string.Join(", ", extensions));
        foreach (var dir in projectDirs)
        {
            Console.WriteLine("Project: " + dir);
            CopyExisting(dir);
            SetupWatcher(dir);
        }
        new ManualResetEvent(false).WaitOne();
    }

    private List<string> GetProjectDirectories(string csproj)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var list = new List<string>();

        void Traverse(string path)
        {
            path = Path.GetFullPath(path);
            if (!seen.Add(path)) return;
            var dir = Path.GetDirectoryName(path)!;
            list.Add(dir);
            var doc = XDocument.Load(path);
            var ns = doc.Root!.Name.Namespace;
            foreach (var pr in doc.Descendants(ns + "ProjectReference"))
            {
                var includeRaw = pr.Attribute("Include")?.Value;
                if (string.IsNullOrWhiteSpace(includeRaw)) continue;
                var inc = includeRaw.Replace('\\', Path.DirectorySeparatorChar);
                Traverse(Path.GetFullPath(Path.Combine(dir, inc)));
            }
        }

        Traverse(csproj);
        return list;
    }

    private void CopyExisting(string dir)
    {
        var files = Directory.EnumerateFiles(dir, "*.*", SearchOption.AllDirectories)
            .Where(f => extensions.Contains(Path.GetExtension(f)) && !IsInBin(f));
        foreach (var f in files)
            Copy(dir, f);
    }

    private void SetupWatcher(string dir)
    {
        if (watchers.Any(w => string.Equals(w.Path, dir, StringComparison.OrdinalIgnoreCase)))
            return;

        var w = new FileSystemWatcher(dir)
        {
            IncludeSubdirectories = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };
        w.Created += (_, e) => DebounceCopy(e.FullPath);
        w.Changed += (_, e) => DebounceCopy(e.FullPath);
        w.Renamed += (_, e) => DebounceCopy(e.FullPath);
        w.EnableRaisingEvents = true;
        watchers.Add(w);
    }

    private void DebounceCopy(string fullPath)
    {
        if (IsInBin(fullPath) || !extensions.Contains(Path.GetExtension(fullPath))) return;
        lock (debounceTimers)
        {
            if (debounceTimers.TryGetValue(fullPath, out var existing))
                existing.Change(debounceInterval, Timeout.InfiniteTimeSpan);
            else
                debounceTimers[fullPath] = new Timer(_ => CopyCallback(fullPath), null, debounceInterval, Timeout.InfiniteTimeSpan);
        }
    }

    private void CopyCallback(string fullPath)
    {
        lock (debounceTimers)
        {
            if (debounceTimers.Remove(fullPath, out var timer))
                timer.Dispose();
        }
        var dir = GetProjectRoot(fullPath);
        Copy(dir, fullPath);
    }

        private bool IsInBin(string path)
    {
        // Skip any path containing a 'bin' folder segment
        var segments = path.Split(Path.DirectorySeparatorChar, StringSplitOptions.RemoveEmptyEntries);
        return segments.Any(s => string.Equals(s, "bin", StringComparison.OrdinalIgnoreCase));
    }


    private string GetProjectRoot(string file)
    {
        var full = Path.GetFullPath(file);
        var match = projectDirs
            .Where(d => full.StartsWith(d + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(d => d.Length)
            .FirstOrDefault();
        if (match == null)
            throw new InvalidOperationException("No project found for " + file);
        return match;
    }

    private void Copy(string dir, string file)
    {
        var rel = Path.GetRelativePath(dir, file);
        var dest = Path.Combine(outputRoot, rel);
        var od = Path.GetDirectoryName(dest)!;
        if (!Directory.Exists(od))
            Directory.CreateDirectory(od);
        try
        {
            File.Copy(file, dest, true);
            Console.WriteLine("Copied: " + rel);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error copying file: " + ex.Message);
        }
    }

    private static string? FindCsprojUpwards(string start)
    {
        var d = new DirectoryInfo(start);
        while (d != null)
        {
            var p = d.GetFiles("*.csproj").FirstOrDefault();
            if (p != null)
                return p.FullName;
            d = d.Parent;
        }
        return null;
    }
}
