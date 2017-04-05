
using System;

namespace FeintSDK{
    public abstract class ClassView{
        protected Request request;

        public ClassView(Request request)
        {
            this.request = request;
        }

        protected abstract Response processRequest();

        public static Func<Request, Response> AsView<T>() where T :ClassView
        {
            return request => ((ClassView)Activator.CreateInstance(typeof(T),request)).processRequest();
        }
    }
}