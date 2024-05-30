namespace TaskTitan.Core.Exceptions;

[Serializable]
public class TaskItemNotFoundException : Exception
{
    public TaskItemNotFoundException()
    {
    }
    public TaskItemNotFoundException(string? message) : base(message)
    {
    }

    public TaskItemNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
