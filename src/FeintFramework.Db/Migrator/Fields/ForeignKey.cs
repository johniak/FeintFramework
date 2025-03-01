
namespace FeintFramework.Db.Migrator.Fields;

public enum ForeignKeyAction
{
    Cascade,
    SetNull,
    SetDefault,
    NoAction,
    Restrict
}

public class ForeignKey : BaseField<int>
{
    public string To { get; set; }

    public ForeignKeyAction OnDelete { get; set; }

    public ForeignKey(string to, ForeignKeyAction onDelete)
    {
        To = to;
        OnDelete = onDelete;
    }
}