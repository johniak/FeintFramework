
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

    public abstract Model? GetById(int id);

    public abstract Form GetForm(string formType, object? obj = null);

    public abstract string GetValue(object obj, string fieldName);

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
        var changePath = $"/{AppName}/{ModelName}/<int:pk>/change";
        var deleteUrlName = $"{AppName}:{ModelName}:delete";
        var deletePath = $"/{AppName}/{ModelName}/<int:pk>/delete";
        var addUrlName = $"{AppName}:{ModelName}:add";
        var addPath = $"/{AppName}/{ModelName}/add";
        AdminUrls.PrefixedUrls.Add(new FeintFramework.Routing.Path(listPath, ListView, listUrlName));
        AdminUrls.PrefixedUrls.Add(new FeintFramework.Routing.Path(changePath, ChangeView, changeUrlName));
    }

    public virtual FeintHttpResponse ChangeView(FeintHttpRequest request)
    {
        var pkString = request.PathParams["pk"];
        var pk = int.Parse(pkString);
        var obj = GetById(pk);
        var form = GetForm("change", obj);
        var context = new Dictionary<string, object>();
        context["form"] = form;
        context["modelName"] = ModelName;
        context["appName"] = AppName;
        return new FeintTemplateResponse("Admin/templates/change.html", context);
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

public abstract class ModelAdmin<T> : ModelAdmin where T : IntModel
{

    class AdminForm : ModelForm<T>
    {
        public AdminForm() : base()
        {
        }
        public AdminForm(T obj) : base(obj)
        {

        }
        static class Meta
        {
            public static string[] Excluded { get; set; } = [nameof(IntModel.Id)];
        }
    }
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


    public override string GetValue(object obj, string fieldName)
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
        if(obj == null)
        {
            return new AdminForm();
        }
        return new AdminForm((T)obj);
    }

    public override Model? GetById(int id)
    {
        var manager = Manager!;
        var result = manager.FirstOrDefault(x => x.Id == id);
        return result;
    }
}