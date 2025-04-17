using System.Collections;
using Microsoft.Extensions.Primitives;

namespace FeintFramework.Http;

public class QueryDict : IEnumerable<KeyValuePair<string, StringValues>>
{
    protected Dictionary<string, StringValues> postDict = new Dictionary<string, StringValues>();

    public QueryDict(IEnumerable<(string Key, StringValues Value)> values)
    {
        foreach (var keyValue in values)
        {
            if (postDict.ContainsKey(keyValue.Key))
            {
                var stringValues = postDict[keyValue.Key];
                var strings = new List<string>(stringValues)
                {
                    keyValue.Value!
                };
                postDict[keyValue.Key] = strings.ToArray();
            }
            else
            {
                postDict[keyValue.Key] = keyValue.Value;
            }
        }
    }


    public StringValues this[string key]
    {
        get => postDict[key];
    }
    public bool ContainsKey(string key)
    {
        return postDict.ContainsKey(key);
    }

    public IEnumerator<KeyValuePair<string, StringValues>> GetEnumerator()
    {
        return postDict.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator Dictionary<string, string>(QueryDict value)
    {
        var dict = new Dictionary<string, string>();
        foreach (var keyValue in value)
        {
            dict[keyValue.Key] = keyValue.Value!;
        }
        return dict;
    }
}