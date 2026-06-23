using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class ManagerViewModel : ObservableObject
{
    private readonly ManagerRepository _repository = new();
    private readonly Staff _manager;

    public string WelcomeMessage => $"Hoş geldiniz, {_manager.FullName}";

    [ObservableProperty] private ObservableCollection<ServiceRequest> _requests = new();
    [ObservableProperty] private ServiceRequest? _selectedRequest;
    [ObservableProperty] private string _managerNote = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;

    public ManagerViewModel(Staff manager)
    {
        _manager = manager;
        LoadRequests();
    }

    private void LoadRequests()
    {
        try
        {
            var list = _repository.GetPendingApproval(_manager.StaffId);
            Requests = new ObservableCollection<ServiceRequest>(list);
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