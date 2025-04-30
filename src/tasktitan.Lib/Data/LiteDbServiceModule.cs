using Jab;

using LiteDB;

using Microsoft.Extensions.Options;

using Raiqub.JabModules.MicrosoftExtensionsOptions;

using TaskTitan.Data;
using TaskTitan.Lib.Parsing;

// [ServiceProviderModule]
// [Singleton<ITaskService, TaskService>]
// [Singleton<IModificationParser, ParlotModificationParser>]
// [Transient<IConfigureOptions<LiteDbOptions>>(Factory = nameof(BindLightDbOptions))]
// [Singleton<LiteDatabase>(Factory = nameof(CreateLiteDb))]
// [Singleton<LiteDbContext>]
// public partial interface ITaskTitanLibModule
// {
//     private static IConfigureOptions<LiteDbOptions> BindLightDbOptions()
//     {
//         return IOptionsModule.Configure<LiteDbOptions>(options => options.DatabaseDirectory = options.DatabaseDirectory);
//     }
//     private static LiteDatabase CreateLiteDb(IOptions<LiteDbOptions> options)
//     {
//         return new LiteDatabase(options.Value.ConnectionString);
//     }
// }