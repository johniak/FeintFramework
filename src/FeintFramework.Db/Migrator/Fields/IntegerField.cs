namespace FeintFramework.Db.Migrator.Fields;

public class IntegerField : BaseField<int>
{
    public bool AutoIncrement { get; set; }
    public IntegerField()
    {
    }
}