namespace FeintFramework.Routing;
public interface IParameterType
{
    string TypeName { get; }
    Type GetInternalType();
    dynamic ToInternalValue(string value);
}
public abstract class ParameterType<T> : IParameterType
{
    public string TypeName { get; protected set; }
    public abstract dynamic ToInternalValue(string value);
    public ParameterType()
    {
        this.TypeName = typeof(T).Name.ToLowerInvariant();
    }

    public Type GetInternalType()
    {
        return typeof(T);
    }
}

public class StringParameter : ParameterType<string>
{
    public StringParameter()
    {
        this.TypeName = "str";
    }

    public override dynamic ToInternalValue(string value)
    {
        return value;
    }
}

public class SlugParameter : StringParameter
{
    public SlugParameter()
    {
        this.TypeName = "slug";
    }
}

public class IntParameter : ParameterType<int>
{
    public IntParameter()
    {
        this.TypeName = "int";
    }

    public override dynamic ToInternalValue(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        throw new FormatException($"Cannot parse '{value}' as int.");
    }
}
