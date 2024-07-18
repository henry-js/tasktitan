using Bogus;

namespace TaskTitan.Cli.Commands.Admin;

public class BogusCommand : Command
{
    public BogusCommand() : base("bogus", "Add dummy tasks")
    {
        AddOptions(this);
    }

    private void AddOptions(Command command)
    {
        var quantityOption = new Option<int>(
            aliases: ["-q", "--quantity"]
        );
        command.AddOption(quantityOption);
    }

    new public class Handler(ITaskItemService service)
    : ICommandHandler
    {
        public int Quantity { get; set; }
        public int Invoke(InvocationContext context)
        {
            return InvokeAsync(context).Result;
        }

        public async Task<int> InvokeAsync(InvocationContext context)
        {
            var tasks = Generate(Quantity);
            foreach (var task in tasks)
            {
                await service.Add(new TaskItemCreateRequest { Task = task });
            };
            return 0;
        }
    }

    public static IEnumerable<TaskItem> Generate(int quantity)
    {
        Randomizer.Seed = new Random(123456);
        var faker = new Faker<TaskItem>()
        .CustomInstantiator(f => TaskItem.CreateNew(f.Hacker.Verb() + " " + f.Hacker.Noun()))
        .RuleFor(t => t.Entry, f => (TaskDate)f.Date.Recent(30))
        .RuleFor(t => t.Modified, (f, t) => f.Date.Soon(1, t.Entry))
        .RuleFor(t => t.Due, (f, t) => f.Date.Future(1, DateTime.Now))
        .RuleFor(t => t.Scheduled, (f, t) => f.Date.Future(1, DateTime.Now))
        .RuleFor(t => t.Status, (f) => f.PickRandom<TaskItemState>(TaskItemState.Values))
        .RuleForType<TaskDate>(typeof(TaskDate), f => f.Date.Soon(50))
        .RuleFor(t => t.Project, f => f.Random.Word());

        return faker.Generate(quantity);
    }
}
