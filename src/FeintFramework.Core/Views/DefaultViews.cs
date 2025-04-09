using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
using FeintFramework.Core.Config;
using FeintFramework.Core.Http;
using FeintFramework.Core.Routing;

namespace FeintFramework.Core.Views;

public record StackData(string? ClassName, string? MethodName, string? File, int Line, int Column, string? HtmlCode, string? LineContent);

public static class DefaultViews
{

    public static FeintHttpResponse NotFound(FeintHttpRequest request, Exception e)
    {
        List<(string Pattern, string? Name)>? patterns = null;
        if (!string.IsNullOrEmpty(e.Message))
            patterns = Router.BuildFullUrlPatternList(Configurator.Settings.RootUrlPatterns.Urls);
        var urlconf = Configurator.Settings.RootUrlPatterns.GetType().FullName;
        return new FeintTemplateResponse("Views/Templates/technical_404.html", new { request, patterns, urlconf, message = e.Message });
    }
    public static FeintHttpResponse ServerError(FeintHttpRequest request, Exception e)
    {
        var stackTrace = new StackTrace(e, true);
        var dotnetVersion = Environment.Version;
        Assembly assembly = Assembly.GetAssembly(typeof(Configurator))!;
        Version frameworkVersion = assembly.GetName().Version!;
        var exceptionName = e.GetType().Name;
        var stackTraceList = new List<StackData>();
        foreach (var frame in stackTrace.GetFrames())
        {
            var method = frame.GetMethod()?.Name;
            var className = frame.GetMethod()?.DeclaringType?.Name;
            var file = frame.GetFileName();
            var line = frame.GetFileLineNumber();
            var columnNumber = frame.GetFileColumnNumber();
            var code = GetHighlightedSource(frame);
            stackTraceList.Add(new StackData(className, method, file, line, columnNumber, code?.html, code?.line));
        }
        if (stackTraceList.Count > 0 && stackTraceList[0].File == null)
        {
            stackTraceList.RemoveAt(0);
        }

        return new FeintTemplateResponse("Views/Templates/technical_500.html", new { request, e, dotnetVersion, frameworkVersion, exceptionName, stackTraceList });
    }

    public static (string html, string line)? GetHighlightedSource(StackFrame frame, int contextLines = 5)
    {
        string? fileName = frame.GetFileName();
        int errorLine = frame.GetFileLineNumber();
        string? errorLineContent = null;
        if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
        {
            return null;
        }
        string[] lines = File.ReadAllLines(fileName);
        int startLine = Math.Max(0, errorLine - contextLines - 1);
        int endLine = Math.Min(lines.Length, errorLine + contextLines);
        int digits = endLine.ToString().Length;
        var sb = new StringBuilder();
        sb.Append("<pre style='font-family: Consolas, monospace; background: #f8f8f8; padding: 10px;'>");
        for (int i = startLine; i < endLine; i++)
        {
            string lineContent = WebUtility.HtmlEncode(lines[i]);
            int displayLineNumber = i + 1;
            if (displayLineNumber == errorLine)
            {
                errorLineContent = lineContent;
                sb.AppendFormat($"<span style='background-color: #ffcccc;'>{{0,{digits}}}: {{1}}</span>\n", displayLineNumber, lineContent);
            }
            else
            {
                sb.AppendFormat($"{{0,{digits}}}: {{1}}\n", displayLineNumber, lineContent);
            }
        }
        sb.Append("</pre>");
        return (sb.ToString(), errorLineContent!);
    }
}