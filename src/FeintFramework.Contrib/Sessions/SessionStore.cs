using System.Text.Json;
using FeintFramework.Contrib.Sessions.Models;

namespace FeintFramework.Contrib.Sessions;

public class SessionStore
{

    protected Session session;
    protected Dictionary<string, object> _data;

    public SessionStore(Session session)
    {
        this.session = session;
        _data = JsonSerializer.Deserialize<Dictionary<string, object>>(session.SessionData)!;
    }

    public object this[string key]
    {
        get
        {
            return _data[key];
        }
        set
        {
            _data[key] = value;
        }
    }

    public T GetValue<T>(string key)
    {
        return (T)_data[key];
    }

    public void SetValue(string key, object value)
    {
        _data[key] = value;
        session.SessionData = JsonSerializer.Serialize(_data);
        session.Save();
    }
}