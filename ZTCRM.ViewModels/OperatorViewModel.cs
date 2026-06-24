using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class OperatorViewModel : ObservableObject
{
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

    public List<string> Priorities { get; } = new() { "Düşük", "Orta", "Yüksek" };
    public List<string> RejectionTypes { get; } = new() { "Eksik Bilgi", "Kapsam Dışı", "Mükerrer", "Diğer" };
    [ObservableProperty] private string _selectedRejectionType = string.Empty;
    [ObservableProperty] private string _rejectionReason = string.Empty;

    public OperatorViewModel(Staff staff)
    {
        _operator = staff;
        LoadRequests();
        LoadCategories();
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

    private void LoadCategories()
    {
        Categories = _repository.GetCategories();
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