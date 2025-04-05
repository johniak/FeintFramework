
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace FeintFramework.Core;

public class Strings
{
    protected List<string> strings { get; set; } = new List<string>();

    public Strings(string text)
    {
        strings.Add(text);
    }

    public Strings(IEnumerable<string> texts)
    {
        strings.AddRange(texts);
    }

    public void Add(string text)
    {
        strings.Add((string)text);
    }

    public void Concat(Strings? values)
    {
        if (values == null)
            return;
        strings.AddRange(values.strings);
    }

    public string GetValueString()
    {
        return string.Join(", ", strings);
    }

    public string[] GetValueArray()
    {
        return strings.ToArray();
    }

    public static bool IsNullOrEmpty(Strings strings)
    {
        if (strings == null)
        {
            return true;
        }
        return !strings.strings.Any(s => !string.IsNullOrEmpty(s));
    }

    public override string ToString()
    {
        return GetValueString();
    }

    public static implicit operator Strings?(string? value)
    {
        if (value == null)
        {
            return null;
        }
        return new Strings(value);
    }

    public static implicit operator Strings?(string[]? values)
    {
        if (values == null)
            return null;
        return new Strings(values);
    }

    public static implicit operator string?(Strings? strings)
    {
        if (strings == null) return null;
        return strings.GetValueString();
    }

    public static implicit operator string[]?(Strings? values)
    {
        if (values == null) return null;
        return values.GetValueArray();
    }
}