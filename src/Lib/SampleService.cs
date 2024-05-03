namespace TaskTitan.Lib.Services;

public class SampleService : ISampleService
{
    private readonly string _message;

    public SampleService(string message)
    {
        _message = message;
    }
    public bool DoWork()
    {
        Console.WriteLine($"Simulating doing work in service. Message from constructor: {_message}");

        return true;
    }

    public interface ITaskRepository
    {
        public void Add(Task task);
        public void Update(Task task);
        public IEnumerable<Task> Get();
        public Task Get(string id);
        public Task Delete(Task task);


    }
}
