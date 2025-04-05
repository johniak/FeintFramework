using System.Threading.Tasks.Dataflow;
using Scriban;
using Scriban.Runtime;
using Scriban.Syntax;

namespace FeintFramework.Core.Http;

public class FeintResponseRedirect : FeintHttpResponse
{


    public FeintResponseRedirect(string redirectTo) : base()
    {
        if (!Uri.TryCreate(redirectTo, UriKind.RelativeOrAbsolute, out var parsedUri))
        {
            throw new ArgumentException($"{redirectTo} is not a valid uri");
        }
        string locationHeaderValue = parsedUri!.IsAbsoluteUri ? parsedUri.AbsoluteUri : parsedUri.ToString();
        Headers["Location"] = locationHeaderValue;
        StatusCode = 302;
    }
}