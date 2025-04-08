using System.Text.Json;
using FeintFramework.Contrib.Sessions.Models;
using Newtonsoft.Json;

namespace FeintFramework.Contrib.Sessions;

public class SessionStore
{

    protected Session session;
    protected Dictionary<string, object> _data;

    public SessionStore(Session session)
    {
        this.session = session;
        _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(session.SessionData)!;
    }

    public object this[string key]
    {
        get
        {
            return _data[key];
        }
        set
        {
            SetValue(key,value);
        }
    }

    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    public T GetValue<T>(string key)
    {
        return (T)_data[key];
    }

    public void SetValue(string key, object value)
    {
        _data[key] = value;
        session.SessionData = System.Text.Json.JsonSerializer.Serialize(_data);
        session.Save();
    }
    public void Remove(string key)
    {
        _data.Remove(key);
        session.SessionData = System.Text.Json.JsonSerializer.Serialize(_data);
        session.Save();
    }
}