using System.Reflection;
using FeintFramework.Db;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Forms.Fields;

namespace FeintFramework.Forms;

public class ModelForm<T> : Form where T : Model
{
    public T? obj { get; protected set; }
    public ModelForm() : base()
    {
    }
    public ModelForm(T obj) : base()
    {
        this.obj = obj;
    }
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
            var allModelFieldNames = getAllModelFieldNames();
            if (getAllFields)
            {
                fieldsToPull = allModelFieldNames;
            }
            else
            {
                fieldsToPull = fieldsNames;
            }
            fieldsToPull!.Where(f => !excludedNames.Contains(f)).ToArray();
            var fieldsFromModel = extractModelFields().Where(f => f.Value.FormField != null)
                .Where(f => fieldsToPull.Contains(f.Key))
                .ToList().Select(f =>
                {
                    var dbField = f.Value;
                    var field = dbField.FormField!;
                    field.Name = f.Key;
                    field.Label = f.Key;
                    if (obj != null)
                    {
                        var value = GetValue(obj, f.Key);
                        field.Initial = value;
                    }
                    return field;
                });

            fields.AddRange(
                fieldsFromModel
            );
            return fields;
        }
    }

    protected static Dictionary<string, BaseField> extractModelFields()
    {
        var dict = new Dictionary<string, BaseField>();

        var props = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            // Grab the first attribute on the property that is a BaseField.
            var fieldAttr = prop
                .GetCustomAttributes(inherit: true)
                .OfType<BaseField>()
                .FirstOrDefault();

            if (fieldAttr != null)
            {
                dict[prop.Name] = fieldAttr;
            }
        }

        return dict;
    }

    public string[] getAllModelFieldNames()
    {
        return extractModelFields().Keys.ToArray();
    }

    protected (string[] Fields, string[] Excluded) extractMeta()
    {
        Type formType = this.GetType();

        Type metaType = formType.GetNestedType(
            "Meta",
            BindingFlags.Public | BindingFlags.NonPublic
        )!;
        if (metaType == null)
        {
            throw new Exception("Meta class not found");
        }
        if (!metaType.IsClass)
        {
            throw new Exception("Meta class must be a class");
        }
        if (metaType.IsGenericType)
        {
            metaType = metaType.MakeGenericType(formType.GenericTypeArguments);
        }
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
    protected string GetValue(object obj, string fieldName)
    {
        var propertyInfo = obj.GetType().GetProperty(fieldName);
        var methodInfo = obj.GetType().GetMethod(fieldName);
        var fieldInfo = obj.GetType().GetField(fieldName);
        if (propertyInfo != null)
        {
            return propertyInfo.GetValue(obj)?.ToString() ?? "";
        }
        else if (methodInfo != null)
        {
            return methodInfo.Invoke(obj, null)?.ToString() ?? "";
        }
        else if (fieldInfo != null)
        {
            return fieldInfo.GetValue(obj)?.ToString() ?? "";
        }
        return "";
    }

    protected void SetValue(object obj, string fieldName, object value)
    {
        var propertyInfo = obj.GetType().GetProperty(fieldName);
        var fieldInfo = obj.GetType().GetField(fieldName);
        if (propertyInfo != null)
        {
            propertyInfo.SetValue(obj, value);
        }
        else if (fieldInfo != null)
        {
            fieldInfo.SetValue(obj, value);
        }
    }

    protected T GetModel()
    {
        if (obj != null)
        {
            return obj;
        }
        var model = Activator.CreateInstance<T>();
        return model;
    }
    public T Save(){
        this.FullClean();
        if (!IsValid)
        {
            throw new ValidationException(Errors);
        }
        var model = GetModel();
        foreach (var field in Fields)
        {
            var value = CleanedData[field.Name];
            SetValue(model, field.Name, value);
        }
        dynamic modelDynamic = model;
        modelDynamic.Save();
        return model;
    }
}