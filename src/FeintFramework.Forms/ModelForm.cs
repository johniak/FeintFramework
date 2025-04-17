using System.Reflection;
using FeintFramework.Core;
using FeintFramework.Core.Http;
using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Forms.Fields;
using static FeintFramework.Core.Shortcuts;

namespace FeintFramework.Forms;

public class ModelForm<T> : Form where T : Model
{
    public override List<BaseFormField> Fields
    {
        get
        {
            var fields = new List<BaseFormField>(base.Fields);
            var (fieldsNames, excludedNames) = extractMeta();
            var excludedClassFields = fields.Any(f => excludedNames.Contains(f.Name));
            if (excludedClassFields)
            {
                throw new Exception("Excluded fields cannot be defined in the class and in the meta class at the same time");
            }
            string[]? fieldsToPull = null;
            var getAllFields = fieldsNames.Length == 0 && excludedNames.Length != 0;
            getAllFields |= fieldsNames.Any(f => f == "__all__");
            var allModelFields = getAllModelFields();
            if (getAllFields)
            {
                fieldsToPull = allModelFields;
            }
            else
            {
                fieldsToPull = fieldsNames;
            }
            fieldsToPull!.Where(f => !excludedNames.Contains(f)).ToArray();

            var type = typeof(T);
            return fields;
        }
    }

    public string[] getAllModelFields()
    {
        var type = typeof(T);
        var fields = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(f => typeof(BaseField).IsAssignableFrom(f.PropertyType))
            .Select(p => p.Name).ToArray();
        return fields;
    }

    protected (string[] Fields, string[] Excluded) extractMeta()
    {
        Type formType = this.GetType();

        Type metaType = formType.GetNestedType(
            "Meta",
            BindingFlags.Public | BindingFlags.NonPublic
        )!;
        var fields = extractMetaFields(metaType);
        var excluded = extractMetaExcluded(metaType);
        if (fields == null && excluded == null)
        {
            throw new Exception(
                "Meta class must have either Fields or Excluded defined"
            );
        }
        return (fields ?? [], excluded ?? []);
    }
    protected string[]? extractMetaExcluded(Type metaType)
    {
        return extractMetaField(metaType, "Excluded");
    }
    protected string[]? extractMetaFields(Type metaType)
    {
        return extractMetaField(metaType, "Fields");
    }

    protected string[]? extractMetaField(Type metaType, string name)
    {
        string[]? fields = null;
        var fieldInfo = metaType.GetField(
            name,
            BindingFlags.Public | BindingFlags.Static
        );

        if (fieldInfo != null && fieldInfo.FieldType == typeof(string[]))
        {
            fields = (string[]?)fieldInfo.GetValue(null);
        }
        else
        {
            var propInfo = metaType.GetProperty(
                name,
                BindingFlags.Public | BindingFlags.Static
            );
            if (propInfo != null && propInfo.PropertyType == typeof(string[]))
            {
                fields = (string[]?)propInfo.GetValue(null);
            }
        }
        return fields;
    }
}