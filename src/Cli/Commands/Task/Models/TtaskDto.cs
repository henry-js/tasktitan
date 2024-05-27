namespace TaskTitan.Cli.Ttask.Models;

public class TTaskDto
{
    public int RowId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Created { get; set; }
    public TTaskState State { get; set; }
    public string? Project { get; set; }
    public DateTime? Due { get; set; }
    public DateTime? Until { get; set; }
    public DateTime? Wait { get; set; }
    public DateTime? End { get; set; }
    public DateTime? Start { get; set; }
    public DateTime? Scheduled { get; set; }
    public DateTime Modified { get; set; }
    public IList<TTaskDto>? Depends { get; set; }
}
