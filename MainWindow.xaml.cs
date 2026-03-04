using System.Windows;
using GtaAdminReportsApp.ViewModels;

namespace GtaAdminReportsApp;

public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;
    }

    private async void LoadReports_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.LoadReportsAsync();
    }

    private async void SortReports_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.SortReportsAsync();
    }

    private void ApplyFilter_Click(object sender, RoutedEventArgs e)
    {
        _viewModel.ApplyFilter();
    }

    private async void ResetDemoData_Click(object sender, RoutedEventArgs e)
    {
        var answer = MessageBox.Show(
            "Пересоздать локальную базу и тестовые данные?",
            "Подтверждение",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (answer != MessageBoxResult.Yes)
        {
            return;
        }

        await _viewModel.ResetDemoDataAsync();
    }

    private async void AddFiftyReports_Click(object sender, RoutedEventArgs e)
    {
        await _viewModel.AddFiftyTestReportsAsync();
    }
}
