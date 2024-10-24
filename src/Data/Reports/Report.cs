using System.Text.Json.Serialization;

using TaskTitan.Data.Expressions;

namespace TaskTitan.Data.Reports;

public class CustomReport : IReport
{
    // public CustomReport()
    // {

    // }
    // public CustomReport(string name)
    // {
    //     Name = name;
    // }
    [JsonIgnore]
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string Filter { get; set; } = string.Empty;
    public string[] Columns { get; set; } = [];
    public string[] Labels { get; set; } = [];

    // TODO: Add support for sorting
}

public interface IReport
{

}

public enum ReportType { Modifiable, Static, Custom }
