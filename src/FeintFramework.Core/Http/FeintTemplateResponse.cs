using System.Threading.Tasks.Dataflow;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace FeintFramework.Core.Http;

public class FeintTemplateResponse : FeintHttpResponse
{

    public FeintTemplateResponse(string templateFilePath, TemplateContext Context) : base()
    {
        initialize(templateFilePath, Context)
    }

    public FeintTemplateResponse(string templateFilePath) : base(){
        var context = new TemplateContext();
        initialize(templateFilePath, context);
    }

    public FeintTemplateResponse(string templateFilePath, Dictionary<string,object> context){
        var scriptObject = new ScriptObject();
            foreach (var item in context)
            {
                scriptObject.Add(item.Key, item.Value); 
            }
            var templateContext = new TemplateContext();
            templateContext.PushGlobal(scriptObject);
    }

    public FeintTemplateResponse(string templateFilePath, object context){

        var template = Template.Parse(File.ReadAllText(templateFilePath));
        Content = template.Render(context);
    }

    protected void initialize(string templateFilePath, TemplateContext Context)
    {

        var template = Template.Parse(File.ReadAllText(templateFilePath));
        Content = template.Render(Context);
    }
}