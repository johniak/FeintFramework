

using System.Runtime.InteropServices.Marshalling;
using FeintFramework.Db;
using LinqToDB;
using LinqToDB.Mapping;

namespace FeintFramework.Db;
public class Model
{
    [PrimaryKey, Identity]
    public int? Id { get; set; }
    // public static ITable<T> Objects<T>() where T : class
    // {
    //     return Connections.Connection!.GetTable<T>();
    // }

    public virtual void Save()
    {
        Console.WriteLine(this);

        if (Id == null)
        {
            Connections.Connection!.Insert(this);
        }
        else
        {
            Connections.Connection!.Update(this);
        }
    }
}