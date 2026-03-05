using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GtaAdminReportsApp.Data;
using GtaAdminReportsApp.DTOs;
using GtaAdminReportsApp.Helpers;
using GtaAdminReportsApp.Models;
using GtaAdminReportsApp.Services;

namespace GtaAdminReportsApp.ViewModels;

public class MainWindowViewModel : INotifyPropertyChanged
{
    private string _statusMessage = "Система готова к работе.";
    private string _selectedAdminClassFilter = "Все";
    private List<ReportViewItem> _allReports = [];

    public ObservableCollection<ReportViewItem> FilteredReports { get; } = [];
    public ObservableCollection<AppLog> Logs { get; } = [];

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
            var logger = new AppLoggerService(db);
            var reportService = new ReportService(db, new ReportClassifierService());
            _allReports = await reportService.GetOpenReportsAsync();
            ReplaceCollection(_allReports);
            StatusMessage = $"Загружено репортов: {_allReports.Count}.";
            await logger.LogInfoAsync("LoadReports", $"Загружено репортов: {_allReports.Count}.");
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки: {ex.Message}";
            await TryWriteErrorLogAsync("LoadReports", ex);
        }
    }

    public async Task SortReportsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.EnsureCreatedAndSeedAsync(db);
            var logger = new AppLoggerService(db);
            var reportService = new ReportService(db, new ReportClassifierService());
            var processedCount = await reportService.SortOpenReportsAsync();
            StatusMessage = $"Сортировка выполнена. Обработано репортов: {processedCount}.";
            await logger.LogInfoAsync("SortReports", $"Сортировка выполнена. Обработано репортов: {processedCount}.");
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сортировки: {ex.Message}";
            await TryWriteErrorLogAsync("SortReports", ex);
        }
    }

    public async Task ResetDemoDataAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.ResetAndSeedAsync(db);
            var logger = new AppLoggerService(db);
            StatusMessage = "Демо-данные пересозданы.";
            await logger.LogInfoAsync("ResetDemoData", "Выполнен сброс и пересоздание демо-данных.");
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка сброса данных: {ex.Message}";
            await TryWriteErrorLogAsync("ResetDemoData", ex);
        }
    }

    public async Task AddFiftyTestReportsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.AddBulkTestReportsAsync(db, 50);
            var logger = new AppLoggerService(db);
            StatusMessage = "Добавлено 50 тестовых репортов.";
            await logger.LogInfoAsync("AddFiftyTestReports", "Добавлено 50 тестовых репортов.");
            await LoadReportsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка добавления тестовых репортов: {ex.Message}";
            await TryWriteErrorLogAsync("AddFiftyTestReports", ex);
        }
    }

    public async Task ApplyFilterAsync()
    {
        var filtered = ReportService.ApplyAdminClassFilter(_allReports, SelectedAdminClassFilter);
        ReplaceCollection(filtered);
        StatusMessage = $"Фильтр применен: {SelectedAdminClassFilter}. Записей: {filtered.Count}.";

        try
        {
            await using var db = DbContextFactory.Create();
            var logger = new AppLoggerService(db);
            await logger.LogInfoAsync("ApplyFilter", $"Фильтр: {SelectedAdminClassFilter}, записей: {filtered.Count}.");
        }
        catch
        {
            // Ошибки логирования не должны прерывать работу интерфейса.
        }
    }

    public async Task LoadLogsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            await DatabaseInitializer.EnsureCreatedAndSeedAsync(db);
            var logger = new AppLoggerService(db);
            var logs = await logger.GetLatestLogsAsync(300);
            ReplaceLogsCollection(logs);
            StatusMessage = $"Загружено логов: {logs.Count}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка загрузки логов: {ex.Message}";
        }
    }

    public async Task ClearLogsAsync()
    {
        try
        {
            await using var db = DbContextFactory.Create();
            var logger = new AppLoggerService(db);
            var deleted = await logger.ClearLogsAsync();
            Logs.Clear();
            StatusMessage = $"Логи очищены. Удалено записей: {deleted}.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Ошибка очистки логов: {ex.Message}";
            await TryWriteErrorLogAsync("ClearLogs", ex);
        }
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

    private void ReplaceLogsCollection(IEnumerable<AppLog> data)
    {
        Logs.Clear();
        foreach (var item in data)
        {
            Logs.Add(item);
        }
    }

    private static async Task TryWriteErrorLogAsync(string action, Exception exception)
    {
        try
        {
            await using var db = DbContextFactory.Create();
            var logger = new AppLoggerService(db);
            await logger.LogErrorAsync(action, exception.Message, exception.ToString());
        }
        catch
        {
            // Если БД недоступна, ошибка логирования игнорируется.
        }
    }
}
