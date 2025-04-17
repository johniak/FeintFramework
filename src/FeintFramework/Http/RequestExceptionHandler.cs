namespace FeintFramework.Http;

public delegate FeintHttpResponse RequestExceptionHandler(FeintHttpRequest request, Exception exception);