
using System.Globalization;

namespace memoryAfteken;


public class CardValueConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isFlipped = (bool)value;
        string cardValue = parameter?.ToString();

        return isFlipped ? cardValue : string.Empty; // Show value if flipped, otherwise empty
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}