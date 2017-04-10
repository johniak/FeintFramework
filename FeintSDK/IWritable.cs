namespace FeintSDK
{
    public interface IWritable : ISavable, IDeletable { }
    public interface ISavable
    {
        object Save();
    }
    public interface IDeletable
    {
        void delete();
    }
}