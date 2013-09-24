using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FeintSDK
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class AOPAttribute:Attribute
    {
        /// <summary>
        /// If returns != null, the main response method will not called
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public abstract Response PreRequest(Request req);

        public abstract void PostRequest(Request req);
    }
}
