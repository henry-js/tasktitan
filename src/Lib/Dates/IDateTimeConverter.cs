namespace TaskTitan.Lib.Dates;

public interface IDateTimeConverter
{
    public DateOnly? ConvertFrom(string value);
}
public interface IStringFilterConverter<T> where T : struct
{
    public DateTime? ConvertFrom(string value);
}
