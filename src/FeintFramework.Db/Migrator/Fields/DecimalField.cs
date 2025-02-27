namespace FeintFramework.Db.Migrator.Fields;

public class DecimalField : BaseField<decimal>
{
    public int MaxDigits { get; set; }
    public int DecimalPlaces { get; set; }
    public DecimalField(string name,int maxDigits, int decimalPlaces) : base(name)
    {
        MaxDigits=maxDigits;
        DecimalPlaces=decimalPlaces;
    }
}