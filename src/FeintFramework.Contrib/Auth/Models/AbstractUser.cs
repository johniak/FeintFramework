using FeintFramework.Db.Migrator.Fields;
using LinqToDB.Mapping;

namespace FeintFramework.Contrib.Auth.Models
{

    public abstract class AbstractUser: FeintFramework.Db.IntModel, IUser
    {
        [Column(Name = "username"), CharField(Length = 255, NotNull = true, Unique = true)]
        public virtual string Username { get; set; }
        [Column(Name = "password"), CharField(Length = 255, NotNull = true)]
        public virtual  string Password { get; protected set; }
        [Column(Name = "email"), CharField(Length = 255, NotNull = true)]
        public  virtual string? Email { get; set; }
        [Column(Name = "is_active"), BooleanField(NotNull = true, DefaultValue = false)]
        public virtual  bool IsActive { get; set; } = false;
        [Column(Name = "is_staff"), BooleanField(NotNull = true, DefaultValue = false)]
        public virtual  bool IsStaff { get; set; } = false;
        [Column(Name = "is_superuser"), BooleanField(NotNull = true, DefaultValue = false)]
        public virtual  bool IsSuperuser { get; set; } = false;
        [Column(Name = "date_joined"), DateTimeField(NotNull = true, AutoNowAdd = true)]
        public virtual  DateTime DateJoined { get; set; }


    }
}