namespace FeintFramework.Db.Migrator.Fields;

public class DateTimeField : BaseField<DateTime>
{
    public bool AutoNowAdd { get; set; }
    public DateTimeField()
    {
    }
}