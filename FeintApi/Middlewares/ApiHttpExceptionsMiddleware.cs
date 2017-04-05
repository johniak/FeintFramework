using System;
using System.Net;
using FeintSDK.Exceptions;
using FeintSDK;
using System.Collections.Generic;

namespace FeintApi.Middlewares
{
    public class CookieSessionMiddleware : Midelware
    {
        public override Response HandleException(Request request, Response response, Url urlApp, Exception ex)
        {
            if (response != null)
                return response;
            if (ex is Http404Exception)
            {
                return new ApiResponse(new Dictionary<string, string>()
                {
                    {"error", "Not Found"}
                }
                ){Status = 404};
            }
            if (ex is Http405Exception)
            {
                return new ApiResponse(new Dictionary<string, string>()
                {
                    {"error", "Method not allowed"}
                }
                ){Status = 405};
            }
            return response;
        }


    }
}