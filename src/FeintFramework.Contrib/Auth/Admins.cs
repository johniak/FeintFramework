
using FeintFramework.Contrib.Admin;

namespace FeintFramework.Contrib.Auth;

public class UserAdmin : ModelAdmin<User>
{
    public override string[]? ListDisplay => new[] { "Username", "Email", "IsActive", "IsStaff", "IsSuperuser" };
    
}