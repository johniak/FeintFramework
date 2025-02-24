using System.Data;
using System.Data.Common;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;

public class MyDbConnection : DataConnection
{
    public MyDbConnection() 
        : base("Microsoft.Data.Sqlite", "Data Source=./MyDatabase.sqlite")
    {
    }

    // // Expose the Blogs table
    // public ITable<Blog> Blogs => this.GetTable<Blog>();
}
