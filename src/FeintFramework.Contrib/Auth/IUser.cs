namespace FeintFramework.Contrib.Auth
{
    public interface IUser
    {
        int? Id { get; set; }
        string? Username { get; set; }
        bool IsActive { get; set; }
        bool IsStaff { get; set; }
        bool IsSuperuser { get; set; }
    }
}