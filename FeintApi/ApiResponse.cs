using System;
using FeintSDK;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace FeintApi
{
    public class ApiResponse : Response
    {
        public ApiResponse(object data)
        {
            var json = JsonConvert.SerializeObject(data);
            initWithText(json);
            ContentType = "application/json; charset=utf-8";
        }

        public ApiResponse(int status)
        {
            Status = status;
            ContentType = "application/json; charset=utf-8";
        }
    }
}