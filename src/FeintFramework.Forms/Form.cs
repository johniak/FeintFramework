using System.Reflection;
using System.Text;
using FeintFramework.Core.Http;
using FeintFramework.Forms.Fields;
using Microsoft.Extensions.Primitives;
using Scriban;
using Scriban.Runtime;

namespace FeintFramework.Forms;

public class Form
{
    public const string NON_FIELD_ERRORS = "__all__";
    public Dictionary<string, string>? Data { get; set; }
    protected Dictionary<string, StringValues>? errors = null;
    public Dictionary<string, StringValues> Errors
    {
        get
        {
            if (errors == null)
            {
                FullClean();
            }
            return errors!;
        }
    }
    public Dictionary<string, UploadedFile> Files { get; set; } = new Dictionary<string, UploadedFile>();
    public string Prefix { get; set; } = "";
    public Dictionary<string, string> Initial { get; set; } = new Dictionary<string, string>();

    public Boolean IsValid
    {
        get
        {
            return Errors.Keys.Count == 0;
        }
    }

    public List<BaseFormField> Fields
    {
        get
        {
            var type = GetType();

            var fields = type
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => typeof(BaseFormField).IsAssignableFrom(f.FieldType))
                .Select(p =>
                {
                    var value = p.GetValue(this) as BaseFormField;
                    if (value != null && string.IsNullOrEmpty(value.Name))
                        value.Name = p.Name;
                    if (Initial.ContainsKey(p.Name))
                    {
                        value.Initial = Initial[p.Name];
                    }
                    return value;
                })
                .Where(value => value != null);

            var properties = type
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => typeof(BaseFormField).IsAssignableFrom(prop.PropertyType))
                                .Select(p =>
                {
                    var value = p.GetValue(this) as BaseFormField;
                    if (value != null && string.IsNullOrEmpty(value.Name))
                        value.Name = p.Name;
                    if (Initial.ContainsKey(p.Name))
                    {
                        value.Initial = Initial[p.Name];
                    }
                    return value;
                })
                .Where(value => value != null);

            return fields.Concat(properties).ToList()!;
        }
    }
    public Dictionary<string, object> CleanedData { get; set; }

    public void FullClean()
    {
        errors = new Dictionary<string, StringValues>();
        CleanedData = new Dictionary<string, object>();
        cleanFields();
        clean();
        postClean();
    }

    protected virtual void cleanFields()
    {
        if (Data == null){
            return;
        }
        foreach (var field in Fields)
        {
            try
            {
                Data.TryGetValue(field.Name, out var value);
                CleanedData[field.Name] = field.Clean(value);
            }
            catch (ValidationException e)
            {
                AddError(e, field.Name);
            }
        }
    }

    protected virtual void clean()
    {

    }

    protected virtual void postClean()
    {

    }

    public void AddError(ValidationException e, string? fieldName = null)
    {
        var key = fieldName ?? NON_FIELD_ERRORS;
        var errorAlreadyExists = Errors!.TryGetValue(key, out var currentErrors);
        StringValues newErrors;
        if (errorAlreadyExists)
            newErrors = new StringValues(currentErrors.Concat(e.Errors).ToArray());
        else
            newErrors = e.Errors;
        errors![key] = newErrors;
    }

    public StringValues? GetFieldError(string fieldName)
    {
        Errors.TryGetValue(fieldName, out var error);
        return error;
    }

    public string AsP
    {
        get
        {
            var context = new TemplateContext();
            var fieldsWithErrors = Fields.Select(f => (f, GetFieldError(f.Name)));
            var scriptObject = new ScriptObject
            {
                { "fields", fieldsWithErrors },
                {"errors", Errors[NON_FIELD_ERRORS]}
            };
            context.PushGlobal(scriptObject);
            var template = Template.Parse(File.ReadAllText("Templates/form_as_p.sbnhtml"));
            return template.Render(context);
        }
    }
}