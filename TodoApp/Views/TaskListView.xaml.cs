using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TodoApp.Models;
using TodoApp.ViewModels;

namespace TodoApp.Views;

public partial class TaskListView : UserControl
{
    public TaskListView()
    {
        InitializeComponent();
    }

    private void SortComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox combo && combo.SelectedItem is ComboBoxItem item
            && item.Tag is string sortBy && DataContext is TaskListViewModel vm)
        {
            vm.SortBy = sortBy;
        }
    }

    private void CategoryFilter_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is Category category
            && DataContext is TaskListViewModel vm)
        {
            vm.SelectedCategoryFilter = category;
        }
    }

    private void DialogOverlay_Click(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is TaskListViewModel vm)
            vm.CancelDialogCommand.Execute(null);
    }

    private void DialogContent_Click(object sender, MouseButtonEventArgs e)
    {
        e.Handled = true; // Zabrání propagaci do overlay
    }
}
