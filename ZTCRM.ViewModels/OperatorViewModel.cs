using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
using Avalonia.Threading;
using System.Linq;
using System.Collections.ObjectModel;

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
    [ObservableProperty] private string _requestsSearchText = string.Empty;
    [ObservableProperty] private string _poolSearchText = string.Empty;
    [ObservableProperty] private ObservableCollection<ServiceRequest> _filteredRequests = new();
    [ObservableProperty] private ObservableCollection<ServiceRequest> _filteredPoolRequests = new();
    [ObservableProperty] private string _newRequestNationalId = string.Empty;
    [ObservableProperty] private string _newRequestDescription = string.Empty;
    [ObservableProperty] private string _newRequestType = string.Empty;
    [ObservableProperty] private string _customerSearchResult = string.Empty;
    [ObservableProperty] private int? _foundCustomerId = null;
    
    public List<string> RequestTypes { get; } = new() { "Şikayet", "Talep" };

    partial void OnRequestsSearchTextChanged(string value) => ApplyRequestsFilter();
    partial void OnRequestsChanged(List<ServiceRequest> value) => ApplyRequestsFilter();
    partial void OnPoolSearchTextChanged(string value) => ApplyPoolFilter();
    partial void OnPoolRequestsChanged(List<ServiceRequest> value) => ApplyPoolFilter();

    private void ApplyRequestsFilter()
    {
        var filtered = Requests.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(RequestsSearchText))
            filtered = filtered.Where(r =>
                r.RequestId.ToString().Contains(RequestsSearchText) ||
                r.CustomerName?.Contains(RequestsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.Description?.Contains(RequestsSearchText, StringComparison.OrdinalIgnoreCase) == true);
        FilteredRequests = new ObservableCollection<ServiceRequest>(filtered);
    }

    private void ApplyPoolFilter()
    {
        var filtered = PoolRequests.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(PoolSearchText))
            filtered = filtered.Where(r =>
                r.RequestId.ToString().Contains(PoolSearchText) ||
                r.CustomerName?.Contains(PoolSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.Description?.Contains(PoolSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.CategoryName?.Contains(PoolSearchText, StringComparison.OrdinalIgnoreCase) == true);
        FilteredPoolRequests = new ObservableCollection<ServiceRequest>(filtered);
    }
    
    public bool IsTab1Selected => SelectedTabIndex == 0;
    public bool IsTab2Selected => SelectedTabIndex == 1;
    public bool IsCustomerFound => FoundCustomerId != null;
    public bool IsCustomerNotFound => !string.IsNullOrEmpty(CustomerSearchResult) && FoundCustomerId == null;


    public List<string> Priorities { get; } = new() { "Düşük", "Orta", "Yüksek" };
    public List<string> RejectionTypes { get; } = new() { "Eksik Bilgi", "Kapsam Dışı", "Mükerrer", "Diğer" };
  
   
    partial void OnFoundCustomerIdChanged(int? value)
    {
        OnPropertyChanged(nameof(IsCustomerFound));
        OnPropertyChanged(nameof(IsCustomerNotFound));
    }

    partial void OnCustomerSearchResultChanged(string value)
    {
        OnPropertyChanged(nameof(IsCustomerFound));
        OnPropertyChanged(nameof(IsCustomerNotFound));
    }
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
    private void SearchCustomer()
    {
        try
        {
            ErrorMessage = string.Empty;
            CustomerSearchResult = string.Empty;
            FoundCustomerId = null;

            if (string.IsNullOrWhiteSpace(NewRequestNationalId))
            {
                ErrorMessage = "TC Kimlik No boş bırakılamaz.";
                return;
            }

            var customer = _repository.FindCustomerByNationalId(NewRequestNationalId);
            if (customer == null)
            {
                CustomerSearchResult = "Müşteri bulunamadı.";
                return;
            }

            FoundCustomerId = customer.CustomerId;
            CustomerSearchResult = $"✓ {customer.FullName}";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
    [RelayCommand]
    private void CreateRequestForCustomer()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (FoundCustomerId == null)
            {
                ErrorMessage = "Lütfen önce müşteri arayın.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewRequestType))
            {
                ErrorMessage = "Başvuru türü seçin.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewRequestDescription))
            {
                ErrorMessage = "Açıklama boş bırakılamaz.";
                return;
            }

            var mappedType = NewRequestType switch
            {
                "Şikayet" => "Complaint",
                "Talep"   => "Request",
                _         => NewRequestType
            };

            _repository.CreateRequest(FoundCustomerId.Value, mappedType, NewRequestDescription, "Phone");
            SuccessMessage = "Başvuru başarıyla oluşturuldu.";
            NewRequestNationalId = string.Empty;
            NewRequestDescription = string.Empty;
            NewRequestType = string.Empty;
            CustomerSearchResult = string.Empty;
            FoundCustomerId = null;
            LoadRequests();
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