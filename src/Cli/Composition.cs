using Pure.DI;
using static Pure.DI.Lifetime;
using Pure.DI.MS;
using TaskTitan.Data;
using TaskTitan.Cli.Display;
using Microsoft.Extensions.Options;
using TaskTitan.Core.Configuration;

internal partial class Composition : ServiceProviderFactory<Composition>
{
    private readonly HostBuilderContext _context;


    public Composition(HostBuilderContext context) : this()
    {
        _context = context;
    }
    void Setup() => DI.Setup()
        .DependsOn(Base)
        .Hint(
            Hint.OnCannotResolveContractTypeNameRegularExpression,
            @"^Microsoft\.(Extensions|AspNetCore)\..+$")
        .Bind().As(Singleton).To(_ => TimeProvider.System)
        .Bind().As(Singleton).To(_ => AnsiConsole.Console)
        .Bind().As(Singleton).To<LiteDbContext>()
        .Bind<ITaskActionHandler>().As(Singleton).To<TaskActionHandler>()
    .Bind<IOptions<TaskTitanConfig>>().To(_ =>
    {
        var config = new TaskTitanConfig();
        // _context.Configuration.GetSection("report").
        _context.Configuration.GetSection("report").Bind(config.Report);
        _context.Configuration.GetSection("uda").Bind(config.Uda);
        return Options.Create<TaskTitanConfig>(config);
    })
        .Bind<IOptions<LiteDbOptions>>().To(_ =>
    {
        var config = new LiteDbOptions();
        // _context.Configuration.GetSection("report").
        config.DatabaseDirectory = Global.DataDirectoryPath;
        return Options.Create<LiteDbOptions>(config);
    })
;
}
