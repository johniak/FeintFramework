using FeintFramework.Db.Migrator.Fields;
using LinqToDB;
using LinqToDB.Mapping;

namespace FeintFramework.Db;

public abstract class Model
{

}

public abstract class IntModel : Model
{
    [PrimaryKey, Identity, AutoField(PrimaryKey = true)]
    public int? Id { get; set; }

}