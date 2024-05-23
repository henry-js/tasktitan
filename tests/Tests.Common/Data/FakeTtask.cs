using Bogus;

using TaskTitan.Core;

namespace TaskTitan.Tests.Common.Data;

public class FakeTtask
{
    public FakeTtask() { }

    public static IEnumerable<TTask> Generate(int quantity)
    {
        Randomizer.Seed = new Random(123456);
        var faker = new Faker<TTask>()
        .CustomInstantiator(f => TTask.CreateNew(f.Lorem.Sentence(3, 2)))
        .RuleFor(t => t.CreatedAt, f => f.Date.Recent(30))
        .RuleFor(t => t.DueDate, (f, t) => f.Date.FutureDateOnly(1));

        return faker.Generate(quantity);
    }
}
