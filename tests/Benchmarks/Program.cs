// See https://aka.ms/new-console-template for more information
using TaskTitan.Data;
using TaskTitan.Tests.Common.Data;


public class EFCoreBenchmarks : TestDatabaseFixture
{
    private readonly TaskTitanDbContext context;

    public EFCoreBenchmarks()
    {
        context = CreateContext();
    }
}
