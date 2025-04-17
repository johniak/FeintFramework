using FeintFramework.Contrib.Sessions;
using FeintFramework.Http;

namespace FeintFramework.Contrib.Auth.Backends;

public abstract class BaseBackend
{
    public BaseBackend()
    {
    }

    public abstract IUser? Authenticate(string username, string password);

    public abstract IUser? GetUser(int userId);

    public void Login(IUser user, FeintHttpRequest request)
    {
        request.AdditionalData[AuthConsts.REQUEST_USER_STORE_KEY] = user;
        var session = request.Session();
        session[AuthConsts.SESSION_USER_ID_KEY] = user.Id!;
    }
}
