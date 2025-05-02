using Vogen;

namespace playground.Infrastructure.Data;

[EfCoreConverter<playground.Core.TaskId>]
[EfCoreConverter<playground.Core.TaskUuid>]
[EfCoreConverter<playground.Core.TaskDescription>]
[EfCoreConverter<playground.Core.ProjectName>]
[EfCoreConverter<playground.Core.EntryDate>]
[EfCoreConverter<playground.Core.ModifiedDate>]
[EfCoreConverter<playground.Core.EndDate>]
[EfCoreConverter<playground.Core.DueDate>]
[EfCoreConverter<playground.Core.WaitDate>]
[EfCoreConverter<playground.Core.ScheduledDate>]
internal partial class VogenEfCoreConverters;