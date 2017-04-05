using System;

namespace FeintSDK
{
    public interface IMidelware
    {
        Request ModifyRequest(Request request);
        Response ModifyResponse(Response response);
        Response IterruptResuest(Request request);
        Response HandleException(Request request, Exception ex);
    }
}