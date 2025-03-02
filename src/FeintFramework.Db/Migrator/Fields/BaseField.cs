namespace FeintFramework.Db.Migrator.Fields;

public class BaseField : Attribute
{
    public bool NotNull { get; set; }
    public bool PrimaryKey { get; set; }
    public bool Unique { get; set; }
    public bool DbIndex { get; set; }

    public virtual object? DefaultValue { get; }

    public BaseField()
    {
    }


}
[AttributeUsage(
		AttributeTargets.Field | AttributeTargets.Property,
		AllowMultiple = true, Inherited = true)]
public class BaseField<T> : BaseField
{
    public T? DefaultValue { get; set; }

    public BaseField()
    {

    }

}

