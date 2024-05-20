namespace TaskTitan.Core;

public class TTaskMetadata
{
    public Priority Priority { get; set; }
    public bool Blocked { get; set; }
    public DateOnly? Wait { get; set; }
    public DateOnly? Scheduled { get; set; }
    public DateOnly? Until { get; set; }

}

public enum Priority { None, Low, Medium, High }
