using System.Text.Json.Serialization;

using TaskTitan.Data.Expressions;

namespace TaskTitan.Data.Reports;

public class CustomReport //: IReport
{
    [JsonIgnore]
    public string Name { get; internal set; }
    public required string Description { get; set; }
    public string Filter { get; set; } = string.Empty;
    public string[] Columns { get; set; } = [];
    public string[] Labels { get; set; } = [];

    public static CustomReport FromFilter(string v)
    {
        throw new NotImplementedException();
    }


    public CustomReport OverrideFilter(params string[] filter)
    {
        Filter = string.Join(' ', filter);

        return this;
    }

    // TODO: Add support for sorting
}

// public interface IReport
// {

// }

public enum ReportType { Modifiable, Static, Custom }
