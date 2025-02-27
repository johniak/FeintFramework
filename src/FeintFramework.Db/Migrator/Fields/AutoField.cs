namespace FeintFramework.Db.Migrator.Fields;

public class AutoField : IntegerField
{
    public AutoField(string name) : base(name)
    {
        AutoIncrement = true;
    }
}