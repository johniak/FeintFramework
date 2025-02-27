using System.Data;
using FeintFramework.Db.Migrator;
using Microsoft.Data.Sqlite;

namespace FeintFramework.Db.Sqlite.Migrator;

class SqliteDatabaseHandler : DatabaseHandler
{
    private SqliteConnection? connection;

    public SqliteDatabaseHandler(string connectionString) : base(connectionString)
    {
        this.MigrationOperationHandler = new SqliteMigrationOperationHandler(this);
    }

    public override void Connect()
    {
        if (connection == null)
        {
            connection = new SqliteConnection(this.connectionString);
        }
        if (connection.State != ConnectionState.Open)
        {
            connection.Open();
        }
    }

    public override void Disconnect()
    {
        if (!(connection != null && connection.State == ConnectionState.Open))
            return;
        connection.Close();
        connection = null;
    }

    public int ExecuteNonQuery(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            command.CommandText = sql;
            return command.ExecuteNonQuery();
        }
    }

    public object? ExecuteScalar(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            command.CommandText = sql;
            return command.ExecuteScalar();
        }
    }

    public DataTable ExecuteQuery(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            command.CommandText = sql;
            using (var reader = command.ExecuteReader())
            {
                DataTable table = new DataTable();
                table.Load(reader);
                return table;
            }
        }
    }
}
