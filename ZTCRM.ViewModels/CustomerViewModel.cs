using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
namespace ZTCRM.ViewModels;

public partial class CustomerViewModel : ObservableObject
{
    private readonly ServiceRequestRepository _repository = new();
    private readonly Customer _customer;
    public List<string> RequestTypes { get; } = new() { "Şikayet", "Talep" };
    [ObservableProperty] private string _requestType = string.Empty;
    [ObservableProperty] private string _description = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private ObservableCollection<ServiceRequest> _requests = new();

    public string WelcomeMessage => $"Hoş geldiniz, {_customer.FullName}";

    public CustomerViewModel(Customer customer)
    {
        _customer = customer;
        LoadRequests();
    }

    private void LoadRequests()
    {
        var list = _repository.GetByCustomer(_customer.CustomerId);
        Requests = new ObservableCollection<ServiceRequest>(list);
    }

    [RelayCommand]
    private void CreateRequest()
    {
       

       
        ErrorMessage = string.Empty;
        SuccessMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(RequestType))
        {
            ErrorMessage = "Şikayet türü boş bırakılamaz.";
            return;
        }
        if (string.IsNullOrWhiteSpace(Description))
        {
            ErrorMessage = "Açıklama boş bırakılamaz.";
            return;
        }

        var mappedType = RequestType switch
        {
            "Şikayet" => "Complaint",
            "Talep"   => "Request",
            _         => RequestType
        };
        var requestId = _repository.Create(_customer.CustomerId, mappedType, Description, null);        SuccessMessage = $"Şikayetiniz #{requestId} numarayla alındı.";
        RequestType = string.Empty;
        Description = string.Empty;
        LoadRequests();
    }
    
    
}