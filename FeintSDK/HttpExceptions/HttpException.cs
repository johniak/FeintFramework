using System;

namespace FeintSDK.Exceptions
{
    public class HttpException : Exception
    {
        public HttpException() : base()
        {

        }
        public HttpException(string message) : base(message)
        {

        }
    }
}