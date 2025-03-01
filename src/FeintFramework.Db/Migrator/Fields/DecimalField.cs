namespace FeintFramework.Db.Migrator.Fields;

public class DecimalField : BaseField<decimal>
{
    public int MaxDigits { get; set; }
    public int DecimalPlaces { get; set; }
    public DecimalField(int maxDigits, int decimalPlaces)
    {
        MaxDigits=maxDigits;
        DecimalPlaces=decimalPlaces;
    }
}