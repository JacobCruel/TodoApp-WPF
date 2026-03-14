using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TodoApp.Models;

namespace TodoApp.Converters;

public class PriorityToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Priority priority)
        {
            return priority switch
            {
                Priority.Low => new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50)),
                Priority.Medium => new SolidColorBrush(Color.FromRgb(0x21, 0x96, 0xF3)),
                Priority.High => new SolidColorBrush(Color.FromRgb(0xFF, 0x98, 0x00)),
                Priority.Critical => new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36)),
                _ => new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E))
            };
        }
        return new SolidColorBrush(Colors.Gray);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
