using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
using Avalonia.Threading;

namespace ZTCRM.ViewModels;

public partial class ManagerViewModel : ObservableObject
{
    private readonly ManagerRepository _repository = new();
    private readonly Staff _manager;
    private readonly DispatcherTimer _refreshTimer;

    public string WelcomeMessage => $"Hoş geldiniz, {_manager.FullName}";

    [ObservableProperty] private List<ServiceRequest> _requests = new();
    [ObservableProperty] private ServiceRequest? _selectedRequest;
    [ObservableProperty] private string _managerNote = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private bool _isRefreshing = false;
    private static async Task NotifyStatusChange(int requestId, string status)
    {
        try
        {
            using var client = new HttpClient();
            var payload = new { requestId, status };
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            await client.PostAsync("http://localhost:5678/webhook/status-change", content);
        }
        catch { } 
    }

    public ManagerViewModel(Staff manager)
    {
        _manager = manager;
        LoadRequests();
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _refreshTimer.Tick += (s, e) => RefreshAll();
        _refreshTimer.Start();
    }
    private async void RefreshAll()
    {
        IsRefreshing = true;
        await Task.Delay(100);
        LoadRequests();
        IsRefreshing = false;
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshAll();
    }

    private void LoadRequests()
    {
        try
        {
            Requests = _repository.GetPendingApproval(_manager.StaffId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Approve()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }
            

            _repository.Approve(SelectedRequest.RequestId, _manager.StaffId, ManagerNote);
            _ = NotifyStatusChange(SelectedRequest.RequestId, "Resolved"); // ekle
            SuccessMessage = $"#{SelectedRequest.RequestId} numaralı başvuru onaylandı.";
            LoadRequests();
            SelectedRequest = null;
            ManagerNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void RejectByManager()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.RejectByManager(SelectedRequest.RequestId, _manager.StaffId, ManagerNote);
            _ = NotifyStatusChange(SelectedRequest.RequestId, "Resolved");
            SuccessMessage = $"#{SelectedRequest.RequestId} numaralı başvuru reddedildi, personele geri döndü.";
            LoadRequests();
            SelectedRequest = null;
            ManagerNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
}