using System.Reflection;
using System.Text;
using FeintFramework.Core.Http;
using FeintFramework.Forms.Fields;
using Scriban;
using Scriban.Runtime;

namespace FeintFramework.Forms;

public class Form
{
    public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();
    public Dictionary<string, UploadedFile> Files { get; set; } = new Dictionary<string, UploadedFile>();
    public string Prefix { get; set; } = "";
    public Dictionary<string, string> Initial { get; set; } = new Dictionary<string, string>();

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
                    return value;
                })
                .Where(value => value != null);

            return fields.Concat(properties).ToList()!;
        }
    }
    public Dictionary<string, object> CleanedData
    {
        get
        {
            Dictionary<string, object> cleanedData = new Dictionary<string, object>();
            foreach (var field in Fields)
            {
                cleanedData[field.Name] = field.toCSharpValue(Data[field.Name]);
            }
            return cleanedData;
        }
    }



    // public string Render()
    // {
    //     StringBuilder builder = new StringBuilder();
    //     foreach (var field in Fields)
    //     {
    //         builder.Append(field.Render());
    //     }

    //     return builder.ToString();
    // }
    public string AsP(){
        var context = new TemplateContext();
        var scriptObject = new ScriptObject
        {
            { "fields", Fields }
        };
        context.PushGlobal(scriptObject);
        var template = Template.Parse(File.ReadAllText("Templates/form_as_p.sbnhtml"));
        return template.Render(context);
    }
}