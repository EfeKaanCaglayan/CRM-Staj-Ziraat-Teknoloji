using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
using Avalonia.Threading;

namespace ZTCRM.ViewModels;

public partial class OperatorViewModel : ObservableObject
{
    private readonly DispatcherTimer _refreshTimer;
    private readonly OperatorRepository _repository = new();
    private readonly Staff _operator;

    public string WelcomeMessage => $"Hoş geldiniz, {_operator.FullName}";

    [ObservableProperty] private List<ServiceRequest> _requests = new();
    [ObservableProperty] private ServiceRequest? _selectedRequest;
    [ObservableProperty] private List<Category> _categories = new();
    [ObservableProperty] private Category? _selectedCategory;
    [ObservableProperty] private string _selectedPriority = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private List<ServiceRequest> _poolRequests = new();
    [ObservableProperty] private ServiceRequest? _selectedPoolRequest;
    [ObservableProperty] private Category? _selectedPoolCategory;
    [ObservableProperty] private string _selectedPoolPriority = string.Empty;
    [ObservableProperty] private int _selectedTabIndex = 0;
    [ObservableProperty] private string _selectedRejectionType = string.Empty;
    [ObservableProperty] private string _rejectionReason = string.Empty;
    [ObservableProperty] private bool _isRefreshing = false;
    
    public bool IsTab1Selected => SelectedTabIndex == 0;
    public bool IsTab2Selected => SelectedTabIndex == 1;

    public List<string> Priorities { get; } = new() { "Düşük", "Orta", "Yüksek" };
    public List<string> RejectionTypes { get; } = new() { "Eksik Bilgi", "Kapsam Dışı", "Mükerrer", "Diğer" };
  
    
    partial void OnSelectedTabIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsTab1Selected));
        OnPropertyChanged(nameof(IsTab2Selected));
        if (value == 0)
            LoadRequests();
        else if (value == 1)
            LoadPoolRequests();
    }
    
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
    
    private void LoadPoolRequests()
    {
        try
        {
            PoolRequests = _repository.GetPool(_operator.StaffId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
    

    public OperatorViewModel(Staff staff)
    {
        _operator = staff;
        LoadRequests();
        LoadCategories();
        LoadPoolRequests();
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _refreshTimer.Tick += (s, e) => RefreshAll();
        _refreshTimer.Start();
    }

    private void LoadRequests()
    {
        try
        {
            Requests = _repository.GetPending();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
    private async void RefreshAll()
    {
        IsRefreshing = true;
        await Task.Delay(1000); 
        if (SelectedTabIndex == 0)
            LoadRequests();
        else
            LoadPoolRequests();
        IsRefreshing = false;
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshAll();
    }

    private void LoadCategories()
    {
        Categories = _repository.GetCategories();
    }
    [RelayCommand]
    private void UpdateCategory()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedPoolRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }
            if (SelectedPoolCategory == null)
            {
                ErrorMessage = "Lütfen bir kategori seçin.";
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedPoolPriority))
            {
                ErrorMessage = "Lütfen öncelik seçin.";
                return;
            }

            var mappedPriority = SelectedPoolPriority switch
            {
                "Düşük"  => "Low",
                "Orta"   => "Medium",
                "Yüksek" => "High",
                _        => SelectedPoolPriority
            };

            _repository.UpdateCategory(SelectedPoolRequest.RequestId, SelectedPoolCategory.CategoryId, mappedPriority);
            SuccessMessage = $"#{SelectedPoolRequest.RequestId} numaralı başvurunun kategorisi güncellendi.";
            LoadPoolRequests();
            SelectedPoolRequest = null;
            SelectedPoolCategory = null;
            SelectedPoolPriority = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
    
    [RelayCommand]
    private void ReturnToPending()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedPoolRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.ReturnToPending(SelectedPoolRequest.RequestId);
            SuccessMessage = $"#{SelectedPoolRequest.RequestId} numaralı başvuru bekleyene gönderildi.";
            LoadPoolRequests();
            LoadRequests();
            SelectedPoolRequest = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CategorizeAndPool()
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
            if (SelectedCategory == null)
            {
                ErrorMessage = "Lütfen bir kategori seçin.";
                return;
            }
            if (string.IsNullOrWhiteSpace(SelectedPriority))
            {
                ErrorMessage = "Lütfen öncelik seçin.";
                return;
            }

            var mappedPriority = SelectedPriority switch
            {
                "Düşük"  => "Low",
                "Orta"   => "Medium",
                "Yüksek" => "High",
                _        => SelectedPriority
            };

            _repository.CategorizeAndPool(SelectedRequest.RequestId, SelectedCategory.CategoryId, mappedPriority, _operator.StaffId);
            SuccessMessage = $"#{SelectedRequest.RequestId} numaralı başvuru kategorize edildi ve havuza eklendi.";
            LoadRequests();
            SelectedRequest = null;
            SelectedCategory = null;
            SelectedPriority = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Reject()
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
            if (string.IsNullOrWhiteSpace(SelectedRejectionType))
            {
                ErrorMessage = "Lütfen red sebebi türü seçin.";
                return;
            }
            if (string.IsNullOrWhiteSpace(RejectionReason))
            {
                ErrorMessage = "Lütfen red sebebi açıklaması girin.";
                return;
            }

            var mappedType = SelectedRejectionType switch
            {
                "Eksik Bilgi" => "MissingInfo",
                "Kapsam Dışı" => "OutOfScope",
                "Mükerrer"    => "Duplicate",
                "Diğer"       => "Other",
                _             => SelectedRejectionType
            };

            _repository.Reject(SelectedRequest.RequestId, mappedType, RejectionReason, _operator.StaffId);
            _ = NotifyStatusChange(SelectedRequest.RequestId, "Rejected"); 
            SuccessMessage = $"#{SelectedRequest.RequestId} numaralı başvuru reddedildi.";
            LoadRequests();
            SelectedRequest = null;
            SelectedRejectionType = string.Empty;
            RejectionReason = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
}