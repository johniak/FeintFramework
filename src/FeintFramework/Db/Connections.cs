using LinqToDB;
using LinqToDB.Data;

namespace FeintFramework.Db;

public class Connections
{
    protected static AsyncLocal<DataConnection?> asyncConnection = new AsyncLocal<DataConnection?>();

    public static DataConnection? Connection
    {
        get => asyncConnection.Value;
        set => asyncConnection.Value = value!;
    }
}
