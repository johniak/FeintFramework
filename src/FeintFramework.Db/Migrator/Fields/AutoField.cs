namespace FeintFramework.Db.Migrator.Fields;

public class AutoField : IntegerField
{
    public AutoField()
    {
        AutoIncrement = true;
    }
}