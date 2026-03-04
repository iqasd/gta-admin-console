using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GtaAdminReportsApp.Data;
using GtaAdminReportsApp.DTOs;
using GtaAdminReportsApp.Helpers;
using GtaAdminReportsApp.Services;

namespace GtaAdminReportsApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "Система готова к работе.";
    private string _selectedAdminClassFilter = "Все";
    private List<ReportViewItem> _allReports = [];

    public ObservableCollection<ReportViewItem> FilteredReports { get; } = [];

    public List<string> AdminClassFilterOptions { get; } = ["Все", "Junior", "Senior", "Head"];

    public string SelectedAdminClassFilter
    {
        get => _selectedAdminClassFilter;
        set
        {
            _selectedAdminClassFilter = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        private set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public async Task LoadReportsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.EnsureCreatedAndSeedAsync(db);
            var reportService = new ReportService(db, new ReportClassifierService());
            _allReports = await reportService.GetOpenReportsAsync();
            ReplaceCollection(_allReports);
            StatusMessage = $"Загружено репортов: {_allReports.Count}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
        }
    }

    public async Task SortReportsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.EnsureCreatedAndSeedAsync(db);
            var reportService = new ReportService(db, new ReportClassifierService());
            var processedCount = await reportService.SortOpenReportsAsync();
            StatusMessage = $"Сортировка выполнена. Обработано репортов: {processedCount}.";
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сортировки: {ex.Message}";
        }
    }

    public async Task ResetDemoDataAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.ResetAndSeedAsync(db);
            StatusMessage = "Демо-данные пересозданы.";
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сброса данных: {ex.Message}";
        }
    }

    public async Task AddFiftyTestReportsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.AddBulkTestReportsAsync(db, 50);
            StatusMessage = "Добавлено 50 тестовых репортов.";
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка добавления тестовых репортов: {ex.Message}";
        }
    }

    public void ApplyFilter()
    {
        var filtered = ReportService.ApplyAdminClassFilter(_allReports, SelectedAdminClassFilter);
        ReplaceCollection(filtered);
        StatusMessage = $"Фильтр применен: {SelectedAdminClassFilter}. Записей: {filtered.Count}.";
    }

    private void ReplaceCollection(IEnumerable<ReportViewItem> data)
    {
        FilteredReports.Clear();
        foreach (var item in data)
        {
            FilteredReports.Add(item);
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
