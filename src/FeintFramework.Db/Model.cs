using FeintFramework.Db.Migrator.Fields;
using LinqToDB;
using LinqToDB.Mapping;

namespace FeintFramework.Db;
public class Model
{
    [PrimaryKey, Identity, AutoField(PrimaryKey = true)]
    public int? Id { get; set; }

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