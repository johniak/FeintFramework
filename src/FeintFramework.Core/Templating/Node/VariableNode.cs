using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace FeintFramework.Core.Templating.Node;

public class VariableNode : BaseNode
{
    public string VariableName { get; }
    public VariableNode(string variableName) => VariableName = variableName;

    public static object? GetVariableValue(string variableName, Dictionary<string, object> context)
    {
        object? value = null;
        var parts = variableName.Split('.');
        if (!context.TryGetValue(parts[0], out value))
            return "";

        for (int i = 1; i < parts.Length; i++)
        {
            if (value == null)
                return "";

            if (value is IDictionary<string, object> dict)
            {
                if (!dict.TryGetValue(parts[i], out value))
                    return "";
            }
            else
            {
                if (int.TryParse(parts[i], out int index))
                {
                    if (value is IList list)
                    {
                        if (index >= 0 && index < list.Count)
                            value = list[index];
                        else
                            return "";
                    }
                    else if (value is IEnumerable enumerable)
                    {
                        int currentIndex = 0;
                        object? found = null;
                        bool foundFlag = false;
                        foreach (var item in enumerable)
                        {
                            if (currentIndex == index)
                            {
                                found = item;
                                foundFlag = true;
                                break;
                            }
                            currentIndex++;
                        }
                        if (!foundFlag)
                            return "";
                        value = found;
                    }
                    else
                    {
                        return "";
                    }
                }
                else
                {
                    Type type = value.GetType();
                    PropertyInfo? property = type.GetProperty(parts[i], BindingFlags.Public | BindingFlags.Instance);
                    if (property != null)
                    {
                        value = property.GetValue(value);
                    }
                    else
                    {
                        FieldInfo? field = type.GetField(parts[i], BindingFlags.Public | BindingFlags.Instance);
                        if (field != null)
                            value = field.GetValue(value);
                        else
                            return "";
                    }
                }
            }
        }
        return value;
    }

    public override string Render(Dictionary<string, object> context)
    {
        object? value = GetVariableValue(VariableName, context);
        return value?.ToString() ?? "";
    }
}
