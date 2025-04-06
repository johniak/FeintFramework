using FeintFramework.Core.Http;
using FeintFramework.Core.Views;

namespace FeintFramework.Core.Routing;

public abstract class RootUrlPatterns : UrlPatterns
{

        public RequestExceptionHandler handler400 {get;}
        public RequestExceptionHandler handler403 {get;}
        public RequestExceptionHandler handler404 {get => DefaultViews.NotFound;}
        public RequestExceptionHandler handler500 {get;}
}