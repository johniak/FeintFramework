using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Auth.Backends;

public class UserModelBackend : BaseBackend
{
    public UserModelBackend(FeintHttpRequest request) : base(request)
    {
    }

    public override bool Authenticate(string username, string password)
    {
        var user =User.Objects.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            return false;
        }
        return user.VerifyPassword(password);
    }

    public override User? GetUser(int userId)
    {
        return User.Objects.FirstOrDefault(u => u.Id == userId);
    }
}