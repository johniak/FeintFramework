using System;

namespace FeintSDK
{
    public abstract class Midelware
    {
        public virtual Request ModifyRequest(Request request, Url urlApp)
        {
            return request;
        }
        public virtual Response ModifyResponse(Request request, Response response, Url urlApp)
        {
            return response;
        }
        public virtual Response IterruptResuest(Request request, Url urlApp)
        {
            return null;
        }

        public virtual Response HandleException(Request request, Response response, Url urlApp, Exception ex)
        {
            return response;
        }
    }
}