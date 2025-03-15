using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Auth.Backends;

public abstract class BaseBackend
{
    public FeintHttpRequest request;
    public BaseBackend(FeintHttpRequest request)
    {
        this.request = request;
    }

    public abstract bool Authenticate(string username, string password);

    public abstract IUser? GetUser(int userId);
}
