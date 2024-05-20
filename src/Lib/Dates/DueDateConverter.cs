namespace TaskTitan.Lib.Dates;

public class DueDateConverter : TypeConverter
{
    readonly StringDateParser _dateParser = new(TimeProvider.System);

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        if (sourceType != typeof(string))
        {
            return false;
        }
        return true;
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        if (value is not string strValue) return base.ConvertFrom(context, culture, value);

        strValue = strValue.Trim().ToLower(CultureInfo.InvariantCulture).Split(':')[^1];

        if (_dateParser.IsRelative(strValue, out DateOnly? relativeDate)) return relativeDate;

        if (_dateParser.IsExactDate(strValue, out DateOnly? exactDate)) return exactDate;

        if (_dateParser.IsNextDate(strValue, out DateOnly? nextDate)) return nextDate;

        // Add more string parsing logic here if needed

        throw new ArgumentException("Unsupported date format.", nameof(value));
    }

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
    {
        if (destinationType == typeof(string))
        {
            return true;
        }
        return base.CanConvertTo(context, destinationType);
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        if (destinationType == typeof(string) && value is DateOnly dateOnly)
        {
            // Convert DateOnly to a string representation, e.g., "2024-05-19"
            return dateOnly.ToString("yyyy-MM-dd", culture);
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }
}
