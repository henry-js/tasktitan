using DbUp;

namespace TaskTitan.Data;

public class DatabaseInitializer
{
    private readonly string _connectionString;

    public DatabaseInitializer(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Initialize()
    {
        var upgrader = DeployChanges.To
            .SQLiteDatabase(_connectionString)
            .WithScriptsEmbeddedInAssembly(typeof(DatabaseInitializer).Assembly)
            .LogToAutodetectedLog()
            .LogToConsole()
            .Build();

        if (upgrader.IsUpgradeRequired())
        {
            var result = upgrader.PerformUpgrade();
            if (result.Successful)
            {
                Console.WriteLine("Database is up to date!");
            }
            else
            {
                Console.WriteLine("Update database failed!");
            }
        }
        else
        {
            Console.WriteLine("No database upgrade required!");
        }
    }
}
