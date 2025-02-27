
using FeintFramework.Core.Config;
using LinqToDB;
using Microsoft.Data.Sqlite;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Sqlite.Migrator;

CharField dateField = new CharField("date") { NotNull = true, Length = 10 };
var fields = new CharField[] { dateField };
SqlFieldRegistry.RegisterField(new SqliteCharField());


Console.WriteLine("***************************************");
// Configurator.Settings = new Example.Core.Settings();
// Configurator.Configure(args);

// using (var db = new MyDbConnection())
// {
//     // This will create the Blogs table if it doesn't exist.
//     // db.CreateTable<Blog>();
//     db.Insert(new Blog { Url = "http://example.com" });
//     Console.WriteLine(db.GetTable<Blog>().Where(b => b.Url == "http://example.com").Count());
// }