using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using TaskTitan.Data.Reports;

namespace TaskTitan.Data.Reports;

public class ReportDictionary : IDictionary<string, CustomReport>
{
    private readonly Dictionary<string, CustomReport> _backingDict = [];
    public CustomReport this[string key]
    {
        get => _backingDict[key]; set
        {
            _backingDict[key] = value;
            SetReportName(key, value);
        }
    }

    public ICollection<string> Keys => _backingDict.Keys;
    public ICollection<CustomReport> Values => _backingDict.Values;
    public int Count => _backingDict.Count;
    public bool IsReadOnly { get; } = false;

    public void Add(string key, CustomReport value)
    {
        _backingDict.Add(key, value);
        SetReportName(key, value);
    }

    public void Add(KeyValuePair<string, CustomReport> item)
    {
        _backingDict.Add(item.Key, item.Value);
        SetReportName(item.Key, item.Value);
    }

    public void Clear() => _backingDict.Clear();

    public bool Contains(KeyValuePair<string, CustomReport> item) => _backingDict.Contains(item);

    public bool ContainsKey(string key) => _backingDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, CustomReport>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, CustomReport>> GetEnumerator()
    {
        return _backingDict.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _backingDict.Remove(key);
    }

    public bool Remove(KeyValuePair<string, CustomReport> item)
    {
        return _backingDict.Remove(item.Key, out var value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out CustomReport value)
    {
        return _backingDict.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => _backingDict.GetEnumerator();

    private static void SetReportName(string key, CustomReport value)
    {
        value.Name = key;
    }
}
