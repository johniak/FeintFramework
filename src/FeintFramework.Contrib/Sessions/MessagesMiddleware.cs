using FeintFramework.Http;
using FeintFramework.Middleware;
using Newtonsoft.Json;

namespace FeintFramework.Contrib.Sessions;

public class MessagesMiddleware : BaseMiddleware
{
    public MessagesMiddleware(RequestHandler handler) : base(handler)
    {
    }

    public override FeintHttpResponse HandleRequest(FeintHttpRequest request)
    {
        var respnonse = handler(request);
        if (request.AdditionalData.ContainsKey(SessionConsts.REQUEST_MESSAGES_KEY))
        {
            var message = request.AdditionalData[SessionConsts.REQUEST_MESSAGES_KEY];
            var messageJson = JsonConvert.SerializeObject(message);
            respnonse.Cookies.Append(SessionConsts.REQUEST_MESSAGES_KEY, messageJson);
        }else{
            respnonse.Cookies.Delete(SessionConsts.REQUEST_MESSAGES_KEY);
        }
        return respnonse;
    }
}