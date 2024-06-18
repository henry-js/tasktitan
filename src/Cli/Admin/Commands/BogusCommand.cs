using System.ComponentModel;
using System.Threading.Tasks;

using Bogus;

using Microsoft.EntityFrameworkCore;

namespace TaskTitan.Cli.AdminCommands;

public class BogusCommand(TaskTitanDbContext dbcontext) : AsyncCommand<BogusCommand.Settings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, Settings settings)
    {
        var deleteTask = dbcontext.Database.ExecuteSqlRawAsync("DELETE FROM tasks");

        var tasks = Generate(settings.Quantity);
        await deleteTask;
        await dbcontext.Tasks.AddRangeAsync(tasks);
        await dbcontext.SaveChangesAsync();
        return 0;
    }

    public class Settings : CommandSettings
    {
        [CommandOption("-q|--quantity")]
        [DefaultValue(10)]
        public int Quantity { get; set; }
    }

    public static IEnumerable<TaskItem> Generate(int quantity)
    {
        Randomizer.Seed = new Random(123456);
        var faker = new Faker<TaskItem>()
        .CustomInstantiator(f => TaskItem.CreateNew(f.Hacker.Verb() + " " + f.Hacker.Noun()))
        .RuleFor(t => t.Entry, f => f.Date.Recent(30))
        .RuleFor(t => t.Due, (f, t) => f.Date.Future(1));

        return faker.Generate(quantity);
    }

}
