namespace FeintFramework.Db.Migrator.Fields;

public partial class CharField : BaseField<string>
{
    public int Length { get; set; }
    public CharField()
    {
    }
}