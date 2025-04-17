namespace FeintFramework.Http.Exceptions;

public class Http404 : Exception
{
    public Http404() : base() { }
    public Http404(string message) : base(message) { }
}