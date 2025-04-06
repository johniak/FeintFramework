namespace FeintFramework.Core.Http;

public delegate FeintHttpResponse RequestExceptionHandler(FeintHttpRequest request, Exception exception);