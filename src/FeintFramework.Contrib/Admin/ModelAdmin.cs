
using System.Reflection;
using FeintFramework.Apps;
using FeintFramework.Http;
using FeintFramework.Db;
using FeintFramework.Forms;
using LinqToDB;
using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Primitives;
using static FeintFramework.Shortcuts;

namespace FeintFramework.Contrib.Admin;

public abstract class ModelAdmin
{
    public abstract string[]? Fields { get; }
    public abstract string[]? Exclude { get; }

    public virtual string[] ListDisplay
    {
        get
        {
            return [nameof(Object.ToString)];
        }
    }

    public abstract string ModelName { get; }

    public abstract Type ModelType { get; }

    public abstract string AppName { get; }

    public abstract Type AppType { get; }

    public virtual string[] ColumnNames
    {
        get
        {
            var columns = new List<string>() { "Id" };
            columns.AddRange(ListDisplay ?? []);
            return columns.ToArray();
        }
    }

    public abstract string[][] GetRows(int page = 0, int pageSize = 25);

    public abstract Form GetForm(string formType, object? obj = null);

    public string ListUrl
    {
        get
        {
            var url = ReverseUrl($"admin:{AppName}:{ModelName}:list")!;
            return url;
        }
    }


    public virtual void Register()
    {
        var listUrlName = $"{AppName}:{ModelName}:list";
        var listPath = $"/{AppName}/{ModelName}";
        var changeUrlName = $"{AppName}:{ModelName}:change";
        var changePath = $"/{AppName}/{ModelName}/<pk:int>/change";
        var deleteUrlName = $"{AppName}:{ModelName}:delete";
        var deletePath = $"/{AppName}/{ModelName}/<pk:int>/delete";
        var addUrlName = $"{AppName}:{ModelName}:add";
        var addPath = $"/{AppName}/{ModelName}/add";
        AdminUrls.PrefixedUrls.Add(new FeintFramework.Routing.Path(listPath, ListView, listUrlName));
    }

    public FeintHttpResponse ListView(FeintHttpRequest request)
    {
        var pageString = request.Query["page"];
        var pageSizeString = request.Query["pageSize"];
        // int? page = !StringValues.IsNullOrEmpty(pageString)?int.Parse(pageString!):null;
        // int? pageSize = !StringValues.IsNullOrEmpty(pageSizeString)?int.Parse(pageSizeString!):null;
        var rows = GetRows();
        var context = new Dictionary<string, object>();
        context["rows"] = rows;
        context["columnNames"] = ColumnNames;
        context["modelName"] = ModelName;
        context["appName"] = AppName;
        return new FeintTemplateResponse("Admin/templates/list.html", context);
    }
}

public abstract class ModelAdmin<T> : ModelAdmin where T : Model
{
    protected T? Object { get; set; }
    protected ITable<T> Manager
    {
        get
        {
            var type = typeof(T);
            var propertyInfo = type.GetProperty(
                "Objects",
                BindingFlags.Public | BindingFlags.Static
            )!;
            return (ITable<T>)propertyInfo.GetValue(null)!;
        }
    }

    public ModelAdmin()
    {

    }

    public override string[]? Fields
    {
        get
        {
            return AdminHelpers.GetModelFields(typeof(T)).Select(f => f.FieldName).ToArray();
        }
    }

    public override string[]? Exclude => [];

    public override string ModelName
    {
        get
        {
            return typeof(T).Name;
        }
    }

    public override Type ModelType
    {
        get
        {
            return typeof(T);
        }
    }

    public override string AppName
    {
        get
        {
            return BaseApplication.FindApplication(typeof(T))!.Name;
        }
    }

    public override Type AppType
    {
        get
        {
            return BaseApplication.FindApplicationType(typeof(T))!;
        }
    }

    public override string[][] GetRows(int page = 0, int pageSize = 25)
    {
        var manager = Manager!;
        var listRows = new List<string[]>();
        var results = manager.Skip(page * pageSize).Take(pageSize);
        foreach (var result in results)
        {
            var row = new string[ColumnNames.Length];
            for (int i = 0; i < ColumnNames.Length; i++)
            {
                var fieldName = ColumnNames[i];
                if (Exclude != null && Exclude.Contains(fieldName))
                {
                    continue;
                }
                row[i] = GetValue(result, fieldName);
            }
            listRows.Add(row);
        }
        return listRows.ToArray();
    }

    public string GetValue(object obj, string fieldName)
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
    public override Form GetForm(string formType, object? obj = null)
    {
        return new Form();
    }

}