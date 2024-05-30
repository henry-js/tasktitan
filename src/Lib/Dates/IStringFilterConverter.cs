namespace TaskTitan.Lib.Dates;

public interface IStringFilterConverter<T> where T : struct
{
    public DateTime? ConvertFrom(string value);
}
