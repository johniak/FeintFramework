
using FeintFramework.Config;
using LinqToDB;
using Microsoft.Data.Sqlite;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Sqlite.Migrator;
using FeintFramework.Contrib.Auth;
using FeintFramework.Contrib.Admin;




Console.WriteLine("***************************************");
Configurator.Settings = new Example.Core.Settings();
Configurator.Migrate();

var appRecords = AdminHelpers.AppRecords;

Configurator.Configure(args);
// using (var db = new MyDbConnection())
// {
//     // This will create the Blogs table if it doesn't exist.
//     // db.CreateTable<Blog>();
//     db.Insert(new Blog { Url = "http://example.com" });
//     Console.WriteLine(db.GetTable<Blog>().Where(b => b.Url == "http://example.com").Count());
// }