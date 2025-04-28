using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace TaskTitan.Data.Reports;


public partial class ReportDictionary : IDictionary<string, ReportDefinition>
{
    private readonly Dictionary<string, ReportDefinition> _backingDict = [];
    public ReportDefinition this[string key]
    {
        get => _backingDict[key]; set
        {
            _backingDict[key] = value;
            SetReportName(key, value);
        }
    }

    public ICollection<string> Keys => _backingDict.Keys;
    public ICollection<ReportDefinition> Values => _backingDict.Values;
    public int Count => _backingDict.Count;
    public bool IsReadOnly { get; } = false;

    public void Add(string key, ReportDefinition value)
    {
        _backingDict.Add(key, value);
        SetReportName(key, value);
    }

    public void Add(KeyValuePair<string, ReportDefinition> item)
    {
        _backingDict.Add(item.Key, item.Value);
        SetReportName(item.Key, item.Value);
    }

    public void Clear() => _backingDict.Clear();

    public bool Contains(KeyValuePair<string, ReportDefinition> item) => _backingDict.Contains(item);

    public bool ContainsKey(string key) => _backingDict.ContainsKey(key);

    public void CopyTo(KeyValuePair<string, ReportDefinition>[] array, int arrayIndex)
    {
        throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, ReportDefinition>> GetEnumerator()
    {
        return _backingDict.GetEnumerator();
    }

    public bool Remove(string key)
    {
        return _backingDict.Remove(key);
    }

    public bool Remove(KeyValuePair<string, ReportDefinition> item)
    {
        return _backingDict.Remove(item.Key, out var value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out ReportDefinition value)
    {
        return _backingDict.TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator() => _backingDict.GetEnumerator();

    private static void SetReportName(string key, ReportDefinition value)
    {
        value.Name = key;
    }
}
