using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TaskTitan.Core.Configuration;

public class ConfigDictionary<T> : IDictionary<string, T>
where T : IConfig
{

    public ConfigDictionary(IEqualityComparer<string>? comparer = null)
    {
        _backingDict = new Dictionary<string, T>(comparer ?? StringComparer.InvariantCultureIgnoreCase);
    }
    private readonly Dictionary<string, T> _backingDict;
    public T this[string key]
    {
        get => _backingDict[key]; set
        {
            _backingDict[key] = value;
            SetReportName(key, value);
        }
    }

    public ICollection<string> Keys => _backingDict.Keys;
    public ICollection<T> Values => _backingDict.Values;
    public int Count => _backingDict.Count;
    public bool IsReadOnly { get; } = false;

    public void Add(string key, T value)
    {
        _backingDict.Add(key, value);
        SetReportName(key, value);
    }

    public void Add(KeyValuePair<string, T> item)
    {
        _backingDict.Add(item.Key, item.Value);
        SetReportName(item.Key, item.Value);
    }

    public void Clear() => _backingDict.Clear();

    public bool Contains(KeyValuePair<string, T> item) => _backingDict.Contains(item);

    public bool ContainsKey(string key) => _backingDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
    {
        return _backingDict.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _backingDict.Remove(key);
    }

    public bool Remove(KeyValuePair<string, T> item)
    {
        return _backingDict.Remove(item.Key, out var value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value)
    {
        return _backingDict.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => _backingDict.GetEnumerator();

    private static void SetReportName(string key, T value)
    {
        value.Name = key;
    }
}
