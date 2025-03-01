namespace FeintFramework.Core.Apps;
public class BaseApplication
{
    public virtual string Name
    {
        get
        {
            return this.GetType().Name;
        }
    }

    public BaseApplication()
    {
    }

    public virtual void Ready()
    {
    }
}