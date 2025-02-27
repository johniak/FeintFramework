namespace FeintFramework.Db.Migrator.Fields;
public class BaseField
{
    public string Name { get; set; }
    public bool NotNull { get; set; }
    public bool PrimaryKey { get; set; }
    public bool Unique { get; set; }
    public bool DbIndex { get; set; }

    public virtual object? DefaultValue { get; }

    public BaseField(string name)
    {
        Name = name;
    }


}
public class BaseField<T> : BaseField
{
    public T? DefaultValue { get; set; }

    public BaseField(string name) : base(name)
    {

    }

}

