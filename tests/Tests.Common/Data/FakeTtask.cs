using Bogus;

using TaskTitan.Core;

namespace TaskTitan.Tests.Common.Data;

public class FakeTaskItem
{
    public FakeTaskItem() { }

    public static IEnumerable<TaskItem> Generate(int quantity)
    {
        Randomizer.Seed = new Random(123456);
        var faker = new Faker<TaskItem>()
        .CustomInstantiator(f => TaskItem.CreateNew(f.Lorem.Sentence(3, 2)))
        .RuleFor(t => t.Entry, f => (TaskDate)f.Date.Recent(30))
        .RuleFor(t => t.Due, (f, t) => f.Date.Future(1));

        return faker.Generate(quantity);
    }
}
