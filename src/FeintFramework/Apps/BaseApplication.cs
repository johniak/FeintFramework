using System.Reflection;

namespace FeintFramework.Apps;
public class BaseApplication
{
    public virtual string Name
    {
        get
        {
            return this.GetType().Name;
        }
    }

    public BaseApplication()
    {
    }

    public virtual void Ready()
    {
    }
    public static BaseApplication? FindApplication(Type startType)
    {
        var type = FindApplicationType(startType);
        if (type == null)
            return null;
        var app = (BaseApplication)Activator.CreateInstance(type)!;
        return app;
    }
    public static Type? FindApplicationType(Type startType)
    {
        var baseType = typeof(BaseApplication);
        Assembly assembly = startType.Assembly;
        string startNamespace = startType.Namespace ?? "";
        var startSegments = startNamespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
        Type? bestCandidate = null;
        int bestCommonSegments = -1;
        int bestTotalSegments = int.MaxValue;

        foreach (var candidate in assembly.GetTypes())
        {
            if (candidate == baseType || !baseType.IsAssignableFrom(candidate))
                continue;

            if (string.IsNullOrEmpty(candidate.Namespace))
                continue;

            var candidateSegments = candidate.Namespace.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            int common = GetCommonPrefixLength(startSegments, candidateSegments);

            if (common > bestCommonSegments || (common == bestCommonSegments && candidateSegments.Length < bestTotalSegments))
            {
                bestCommonSegments = common;
                bestTotalSegments = candidateSegments.Length;
                bestCandidate = candidate;
            }
        }

        return bestCandidate;
    }
    private static int GetCommonPrefixLength(string[] seg1, string[] seg2)
    {
        int len = Math.Min(seg1.Length, seg2.Length);
        int count = 0;
        for (int i = 0; i < len; i++)
        {
            if (seg1[i] == seg2[i])
                count++;
            else
                break;
        }
        return count;
    }
}