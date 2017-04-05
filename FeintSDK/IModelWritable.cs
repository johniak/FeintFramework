namespace FeintSDK
{
    public interface IModelWritable : IModelSavable, IModelDeletable { }
    public interface IModelSavable
    {
        void Save();
    }
    public interface IModelDeletable
    {
        void delete();
    }
}