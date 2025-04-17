using FeintFramework.Forms.Fields;

namespace FeintFramework.Db.Migrator.Fields;

public class AutoField : IntegerField
{
    public AutoField()
    {
        AutoIncrement = true;
        NotNull = true;
    }
    public override BaseFormField? FormField => null;
}