using System.Data;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Sql;
using Microsoft.Data.Sqlite;

namespace FeintFramework.Db.Sqlite.Migrator;

public class SqliteDatabaseHandler : DatabaseHandler
{
    private SqliteConnection? connection;
    private SqliteTransaction? outerTransaction;
    private int transactionLevel = 0;

    public SqliteDatabaseHandler(string connectionString) : base(connectionString)
    {
        this.MigrationOperationHandler = new SqliteMigrationOperationHandler(this);
        InitRegistries();
    }

    protected void InitRegistries()
    {
        SqlFieldRegistry.RegisterField(new SqliteBinaryField());
        SqlFieldRegistry.RegisterField(new SqliteBooleanField());
        SqlFieldRegistry.RegisterField(new SqliteCharField());
        SqlFieldRegistry.RegisterField(new SqliteDateField());
        SqlFieldRegistry.RegisterField(new SqliteDateTimeField());
        SqlFieldRegistry.RegisterField(new SqliteDecimalField());
        SqlFieldRegistry.RegisterField(new SqliteFloatField());
        SqlFieldRegistry.RegisterField(new SqliteIntegerField());
        SqlFieldRegistry.RegisterField(new SqliteTextField());
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
        Dispose();
        connection.Close();
        connection = null;
    }

    public int ExecuteNonQuery(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            Console.WriteLine(sql);
            return 0;
            if (outerTransaction != null)
                command.Transaction = outerTransaction;
            command.CommandText = sql;
            return command.ExecuteNonQuery();
        }
    }

    public object? ExecuteScalar(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            if (outerTransaction != null)
                command.Transaction = outerTransaction;
            command.CommandText = sql;
            return command.ExecuteScalar();
        }
    }

    public DataTable ExecuteQuery(string sql)
    {
        using (var command = connection!.CreateCommand())
        {
            if (outerTransaction != null)
                command.Transaction = outerTransaction;
            command.CommandText = sql;
            using (var reader = command.ExecuteReader())
            {
                DataTable table = new DataTable();
                table.Load(reader);
                return table;
            }
        }
    }

    public override void BeginTransaction()
    {
        if (transactionLevel == 0)
        {
            this.outerTransaction = connection?.BeginTransaction()!;
        }
        else
        {
            transactionLevel++;
            string savepointName = "SP" + transactionLevel;
            this.ExecuteNonQuery($"SAVEPOINT {savepointName};");
        }
    }

    public override void CommitTransaction()
    {
        if (transactionLevel == 0)
            throw new InvalidOperationException("No active transaction to commit.");

        if (transactionLevel == 1)
        {
            outerTransaction.Commit();
            outerTransaction.Dispose();
            outerTransaction = null;
            transactionLevel = 0;
        }
        else
        {
            string savepointName = "SP" + transactionLevel;
            this.ExecuteNonQuery($"RELEASE SAVEPOINT {savepointName};");
            transactionLevel--;
        }
    }

    public override void RollbackTransaction()
    {
        if (transactionLevel == 0)
            throw new InvalidOperationException("No active transaction to rollback.");

        if (transactionLevel == 1)
        {
            outerTransaction.Rollback();
            outerTransaction.Dispose();
            outerTransaction = null;
            transactionLevel = 0;
        }
        else
        {
            // Roll back to the nested savepoint, then release it.
            string savepointName = "SP" + transactionLevel;
            this.ExecuteNonQuery($"ROLLBACK TO SAVEPOINT {savepointName}; RELEASE SAVEPOINT {savepointName};");
            transactionLevel--;
        }
    }

    public void Dispose()
    {
        if (transactionLevel > 0)
        {
            try
            {
                while (transactionLevel > 0)
                {
                    RollbackTransaction();
                }
            }
            catch { }
        }
        connection?.Dispose();
    }


}
