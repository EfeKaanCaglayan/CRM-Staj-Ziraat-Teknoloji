using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class StaffViewModel : ObservableObject
{
    private readonly StaffRequestRepository _repository = new();
    private readonly Staff _staff;

    public string WelcomeMessage => $"Hoş geldiniz, {_staff.FullName} | {_staff.UnitName ?? "Birim atanmadı"}";

    [ObservableProperty] private ObservableCollection<ServiceRequest> _poolRequests = new();
    [ObservableProperty] private ObservableCollection<ServiceRequest> _myRequests = new();
    [ObservableProperty] private ServiceRequest? _selectedPoolRequest;
    [ObservableProperty] private ServiceRequest? _selectedMyRequest;
    [ObservableProperty] private string _resolutionNote = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;

    public StaffViewModel(Staff staff)
    {
        _staff = staff;
        LoadPool();
        LoadMyRequests();
    }

    private void LoadPool()
    {
        try
        {
            var list = _repository.GetPool(_staff.StaffId);
            PoolRequests = new ObservableCollection<ServiceRequest>(list);
        
          
            if (list.Count == 0) Console.WriteLine("DEBUG: Pool sorgusu 0 kayıt döndürdü.");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"DEBUG HATASI: {ex.Message}";
            Console.WriteLine($"DEBUG: {ex.ToString()}");
        }
    }

    private void LoadMyRequests()
    {
        try
        {
            var list = _repository.GetMyRequests(_staff.StaffId);
            MyRequests = new ObservableCollection<ServiceRequest>(list);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void AssignToSelf()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedPoolRequest == null)
            {
                ErrorMessage = "Lütfen havuzdan bir başvuru seçin.";
                return;
            }

            _repository.AssignToSelf(SelectedPoolRequest.RequestId, _staff.StaffId);
            SuccessMessage = $"#{SelectedPoolRequest.RequestId} numaralı başvuru üzerinize alındı.";
            LoadPool();
            LoadMyRequests();
            SelectedPoolRequest = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Resolve()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.UpdateStatus(SelectedMyRequest.RequestId, "Resolved", _staff.StaffId, ResolutionNote);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru çözüldü olarak işaretlendi.";
            LoadMyRequests();
            SelectedMyRequest = null;
            ResolutionNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CannotResolve()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ResolutionNote))
            {
                ErrorMessage = "Çözülemez durumunda açıklama zorunludur.";
                return;
            }

            _repository.UpdateStatus(SelectedMyRequest.RequestId, "CannotResolve", _staff.StaffId, ResolutionNote);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru çözülemez olarak işaretlendi.";
            LoadMyRequests();
            SelectedMyRequest = null;
            ResolutionNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ReturnToPool()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.ReturnToPool(SelectedMyRequest.RequestId, _staff.StaffId);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru havuza geri gönderildi.";
            LoadPool();
            LoadMyRequests();
            SelectedMyRequest = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
}