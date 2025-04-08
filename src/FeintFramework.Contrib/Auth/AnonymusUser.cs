
namespace FeintFramework.Contrib.Auth
{
    public class AnonymusUser : IUser
    {
        public int? Id { get; set; } = null;
        public string? Username { get; set; } = null;
        public bool IsActive { get; set; } = false;
        public bool IsStaff { get; set; } = false;
        public bool IsSuperuser { get; set; } = false;
        public bool IsAuthenticated { get; set; } = false;
    }
}