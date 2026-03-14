using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace TodoApp.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }
}

// Inline converter pro progress bar šířku
public class PercentWidthConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length >= 2 && values[0] is double percent && values[1] is double containerWidth)
            return Math.Max(0, containerWidth * Math.Min(percent, 100) / 100.0);
        return 0.0;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
