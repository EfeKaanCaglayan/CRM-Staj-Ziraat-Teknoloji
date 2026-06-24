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
    [ObservableProperty] private string _aiMessage = string.Empty;
    [ObservableProperty] private bool _isApproved = false;
    [ObservableProperty] private ServiceRequest? _selectedRequest;
    
    
    
    private readonly GroqService _groqService = new();
    [RelayCommand]
    private async Task GetApproval()
    {
        if (string.IsNullOrWhiteSpace(Description))
        {
            AiMessage = "Lütfen önce açıklama girin.";
            return;
        }

        AiMessage = "Kontrol ediliyor...";
        IsApproved = false;

        var messages = new List<(string role, string content)>
        {
            ("user", $"Müşteri adı: {_customer.FullName}\nŞikayet türü: {RequestType}\nAçıklama: {Description}")
        };

        var response = await _groqService.SendMessageAsync(messages);
        AiMessage = response;

        if (response.Contains("Bilgiler tam"))
            IsApproved = true;
    }

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
        if (!IsApproved)
        {
            ErrorMessage = "Lütfen önce 'Onay Al' butonuna basın.";
            return;
        }
   
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

        var requestId = _repository.Create(_customer.CustomerId, mappedType, Description, null);
        SuccessMessage = $"Şikayetiniz #{requestId} numarayla alındı.";
        RequestType = string.Empty;
        Description = string.Empty;
        IsApproved = false;
        AiMessage = string.Empty;
        LoadRequests();
    }

    [RelayCommand]
    private void CancelRequest()
    {
        try
        {
                ErrorMessage=string.Empty;
                SuccessMessage=string.Empty;
                if (SelectedRequest==null)
                {
                    ErrorMessage = "Lütfen başvuru seçin.";
                    return;
                }

                if (SelectedRequest.CurrentStatus == "Kapatıldı" || 
                    SelectedRequest.CurrentStatus == "İptal" || 
                    SelectedRequest.CurrentStatus == "Reddedildi")
                {
                    ErrorMessage = "Bu başvuru iptal edilemez.";
                    return;
                }
                _repository.Cancel(SelectedRequest.RequestId,_customer.CustomerId);
                SuccessMessage = $"#{SelectedRequest.RequestId} numaralı başvuru iptal edildi.";
                SelectedRequest = null;
                LoadRequests();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }


}