using System.Globalization;

namespace memoryAfteken;

public class BooleanToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isFlipped)
        {
            return isFlipped ? "LightGray" : "DarkGreen"; // Example colors
        }
        return "DarkGreen";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}