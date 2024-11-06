using TaskTitan.Core.Configuration;

namespace TaskTitan.Data;

public class LiteDbOptions
{

    public string FileName = "tasktitan.db";
    public string DatabaseDirectory { get; set; } = Global.DataDirectoryPath;
    public string Connection { get; set; } = "shared";
    public string ConnectionString => $"{nameof(FileName)}={Path.Combine(DatabaseDirectory, FileName)};{nameof(Connection)}={Connection}";
}
