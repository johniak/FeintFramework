using System;

namespace FeintSDK
{
    public interface IMidelware
    {
        Request ModifyRequest(Request request, Url urlApp);
        Response ModifyResponse(Request request, Response response, Url urlApp);
        Response IterruptResuest(Request request, Url urlApp);
        Response HandleException(Request request, Response response, Url urlApp, Exception ex);
    }
}