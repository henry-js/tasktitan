namespace TaskTitan.Cli.Reports;

public class TaskTitanOptions
{
    public Dictionary<ReportOptions.BuiltIn, ReportOptions> Report { get; } = new();
    // {
    //     {
    //         BuiltIn.list, new(){
    //             Description = "Pending tasks",
    //             Columns = "id,start.age,entry.age,depends.indicator,priority,project,tags,recur.indicator,scheduled.countdown,due,until.remaining,description.count,urgency",
    //             Filter = "status:pending",
    //             Labels = "ID,Active,Age,D,P,Project,Tags,R,Sch,Due,Until,Description,Urg",
    //             Sort = "start-,due+,project+,urgency-",
    //         }
    //     },
    //     {
    //         BuiltIn.all, new() {
    //             Description = "All tasks",
    //             Columns = "id,status.short,uuid.short,start.active,entry.age,end.age,depends.indicator,priority,project.parent,tags.count,recur.indicator,wait.remaining,scheduled.remaining,due,until.remaining,description",
    //             Filter = "",
    //             Labels = "ID,St,UUID,A,Age,Done,D,P,Project,Tags,R,Wait,Sch,Due,Until,Description",
    //             Sort = "entry-"
    //         }
    //     }
    // };
}

public class ReportOptions
{
    public enum BuiltIn
    {
        list,
        all
    }
    public string Description { get; set; }
    public string Columns { get; set; }
    public string Labels { get; set; }
    public string Sort { get; set; }
    public string Filter { get; set; }
}
