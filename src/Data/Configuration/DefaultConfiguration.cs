using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TaskTitan.Data;
using TaskTitan.Data.Reports;
using static TaskTitan.Data.Enums.ColFormat;
using static TaskTitan.Data.Enums.ColType;

namespace TaskTitan.Configuration;

public class ReportConfiguration
{
    [DataMember(Name = "Report")]
    public ReportDictionary Report { get; set; } = [];
    public ReportConfiguration()
    {
    }

    [IgnoreDataMember]
    public static Dictionary<string, AttributeColumnConfig> Columns => new(){

        {"depends", new AttributeColumnConfig("depends",true, List,Text,  [Standard,Indicator,List] )},
        {"description", new AttributeColumnConfig("description",true, Standard,Text,  [Combined,Desc,Oneline,Truncated, Count, TruncatedCount] )},
        {"due", new AttributeColumnConfig("due",true, Formatted,Date)},
        {"end", new AttributeColumnConfig("end",true, Formatted,Date )},
        {"entry", new AttributeColumnConfig("entry",true, Formatted,Date )},
        {"estimate", new AttributeColumnConfig("estimate",true, Standard,Text,  [Standard,Indicator] )},
        {"modified", new AttributeColumnConfig("modified",true, Formatted,Date )},
        {"parent", new AttributeColumnConfig("parent",false, Long,Text ,[Long, Short])},
        {"project", new AttributeColumnConfig("project",false, Full,Text, [ Full, Parent, Indented ] )},
        {"recur", new AttributeColumnConfig("recur",false, Full,Text, [ Duration, Indicator ] )},
        {"scheduled", new AttributeColumnConfig("scheduled",true, Formatted,Date )},
        {"start", new AttributeColumnConfig("start",true, Formatted,Date )},
        {"status", new AttributeColumnConfig("status",true, Long, Text,[Long, Short] )},
        {"tags", new AttributeColumnConfig("tags",true, List, Text,[List,Indicator, Count] )},
        {"until", new AttributeColumnConfig("until",true, Formatted,Date )},
        {"uuid", new AttributeColumnConfig("uuid",false, Long,Text, [Long, Short] )},
        {"wait", new AttributeColumnConfig("wait",true, Standard,Date )},
    };
}
