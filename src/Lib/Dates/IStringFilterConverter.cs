namespace TaskTitan.Lib.Dates;

public class AttributeFilterConversionOptions : IExpressionConversionOptions
{
    public static AttributeFilterConversionOptions Default { get; } = new();
    public string[] StandardDateAttributes { get; } = ["due", "scheduled", "created", "modified", "started", "wait"];
    public string[] StandardStringAttributes { get; } = ["status", "project"];
    public IStringFilterConverter<DateTime> StandardDateConverter { get; set; } = new DateTimeConverter(TimeProvider.System);
}
