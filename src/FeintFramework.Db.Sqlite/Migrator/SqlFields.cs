using System.Globalization;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Sql;
namespace FeintFramework.Db.Migrator;

public abstract class BaseSqliteField<T> : SqlField<T> where T : BaseField
{
    public override List<string> GetSqlAttributes(T field)
    {
        List<string> attributes = new List<string>();
        if (field.PrimaryKey)
        {
            attributes.Add("PRIMARY KEY");
        }
        if (field.NotNull)
        {
            attributes.Add("NOT NULL");
        }
        if (field.Unique)
        {
            attributes.Add("UNIQUE");
        }
        if (field.DbIndex)
        {
            attributes.Add("INDEX");
        }
        if (this.GetSqlDefaultValue(field) != null)
        {
            attributes.Add($"DEFAULT {field.DefaultValue}");
        }
        return attributes;
    }
}


class SqliteBinaryField : BaseSqliteField<BinaryField>
{
    public override string GetSqlType(BinaryField field)
    {
        return $"BLOB";
    }
}

class SqliteBooleanField : BaseSqliteField<BooleanField>
{
    public override string GetSqlType(BooleanField field)
    {
        return $"BOOLEAN";
    }
}

public class SqliteCharField : BaseSqliteField<CharField>
{
    public override string GetSqlType(CharField field)
    {
        return $"VARCHAR({field.Length})";
    }
}

public class SqliteDateField : BaseSqliteField<DateField>
{
    public override string GetSqlType(DateField field)
    {
        return $"DATE";
    }
    public override string? GetSqlDefaultValue(DateField field)
    {
        if (field.DefaultValue == null)
            return null;
        return field.DefaultValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
    }
}

public class SqliteDateTimeField : BaseSqliteField<DateTimeField>
{
    public override string GetSqlType(DateTimeField field)
    {
        return $"DATETIME";
    }
    public override string? GetSqlDefaultValue(DateTimeField field)
    {
        if (field.AutoNowAdd)
            return "CURRENT_TIMESTAMP";
        if (field.DefaultValue == null)
            return null;
        return field.DefaultValue.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

}

public class SqliteDecimalField : BaseSqliteField<DecimalField>
{
    public override string GetSqlType(DecimalField field)
    {
        return $"DECIMAL({field.MaxDigits},{field.DecimalPlaces})";
    }
}

public class SqliteFloatField : BaseSqliteField<FloatField>
{
    public override string GetSqlType(FloatField field)
    {
        return $"REAL";
    }
}
public class SqliteIntegerField : BaseSqliteField<IntegerField>
{
    public override string GetSqlType(IntegerField field)
    {
        return $"INTEGER";
    }
}
public class SqliteTextField : BaseSqliteField<TextField>
{
    public override string GetSqlType(TextField field)
    {
        return $"TEXT";
    }
}

public class SqliteForeignKey : BaseSqliteField<ForeignKey>
{
    public override string GetSqlType(ForeignKey field)
    {
        return $"INTEGER";
    }
    public override List<string> GetSqlAttributes(ForeignKey field)
    {
        List<string> attributes = ["REFERENCES"];
        var referenceTable = field.To.Replace(".", "").ToUnderscoreCase();
        attributes.Add(referenceTable);
        attributes.Add($"(id)");
        attributes.Add($"ON DELETE {field.OnDelete}");
        return attributes;
    }
}
