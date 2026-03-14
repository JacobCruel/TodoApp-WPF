using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class CalendarView : UserControl
{
    public CalendarView()
    {
        InitializeComponent();
    }

    private void AddTaskButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement element && element.Tag is CalendarDayViewModel day)
        {
            var mainWindow = Window.GetWindow(this);
            if (mainWindow?.DataContext is MainViewModel mainVm)
            {
                mainVm.AddTaskForDate(day.Date);
            }
        }
    }
}
