using FeintFramework.Core.Http;

namespace FeintFramework.Contrib.Auth.Backends;

public class UserModelBackend : BaseBackend
{
    public UserModelBackend() : base()
    {
    }

    public override User? Authenticate(string username, string password)
    {
        var user = User.Objects.FirstOrDefault(u => u.Username == username);
        if (user == null) return null;
        var passwordCorrect = user.VerifyPassword(password);
        if (!passwordCorrect) return null;
        return user;
    }

    public override User? GetUser(int userId)
    {
        return User.Objects.FirstOrDefault(u => u.Id == userId);
    }
}