namespace TaskTitan.Lib.Dates;

public interface IDateTimeConverter
{
    public DateOnly? ConvertFrom(string value);
}
