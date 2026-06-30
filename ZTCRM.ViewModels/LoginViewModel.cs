using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ZTCRM.Data;
using ZTCRM.Models;
using System.Linq;
using System.Net.Http.Json;

namespace ZTCRM.ViewModels;


public partial class LoginViewModel : ObservableObject
{
    
   
    private readonly StaffRepository _staffRepository = new StaffRepository();
    private readonly CustomerRepository _customerRepository = new CustomerRepository();
    public event Action<Staff>? LoginSuccessful;
     public event Action<Customer>? CustomerLoginSuccessful;
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    [ObservableProperty]
    private bool _isPersonelTab = true;

    [ObservableProperty]
    private bool _isMusteriTab = false;

    [ObservableProperty]
    private bool _isYabanciMusteri = false;
    [ObservableProperty] private bool _isKvkkAccepted = false;

    public bool IsTcknVisible => IsMusteriTab && !IsYabanciMusteri;

    partial void OnUsernameChanged(string value)
    {
        if (IsMusteriTab && !IsYabanciMusteri && !string.IsNullOrEmpty(value))
        {
            var filtered = new string(value.Where(char.IsDigit).ToArray());
            if (filtered != value)
                Username = filtered;
        }
    }

    partial void OnIsYabanciMusteriChanged(bool value)
    {
        Username = string.Empty;
        OnPropertyChanged(nameof(IsTcknVisible));
    }

    partial void OnIsMusteriTabChanged(bool value)
    {
        OnPropertyChanged(nameof(IsTcknVisible));
    }

    [RelayCommand]
    private void SelectPersonel()
    {
        IsPersonelTab = true;
        IsMusteriTab = false;
        IsYabanciMusteri = false;
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void SelectMusteri()
    {
        IsPersonelTab = false;
        IsMusteriTab = true;
        IsYabanciMusteri = false;
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }
    [RelayCommand]
private async Task Login()
{
    ErrorMessage = string.Empty;

    if (IsPersonelTab)
    {
        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Kullanıcı adı ve şifre boş bırakılamaz.";
            return;
        }

        var staff = _staffRepository.Login(Username, Password);
        if (staff == null)
        {
            ErrorMessage = "Kullanıcı adı veya şifre hatalı.";
            return;
        }

        OnLoginSuccess(staff);
    }
    else
    {
        if (!IsKvkkAccepted)
        {
            ErrorMessage = "Lütfen KVKK aydınlatma metnini kabul edin.";
            return;
        }

    

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = IsYabanciMusteri
                ? "Pasaport No boş bırakılamaz."
                : "TC Kimlik No boş bırakılamaz.";
            return;
        }

        if (!IsYabanciMusteri && Username.Length != 11)
        {
            ErrorMessage = "TC Kimlik No 11 haneli olmalıdır.";
            return;
        }

        var customer = IsYabanciMusteri
            ? _customerRepository.Login(null, Username)
            : _customerRepository.Login(Username);

        if (customer == null)
        {
            ErrorMessage = IsYabanciMusteri
                ? "Pasaport No bulunamadı."
                : "TC Kimlik No bulunamadı.";
            return;
        }

        OnCustomerLoginSuccess(customer);
    }
}

private async Task<bool> VerifyCaptcha()
{
    try
    {
        using var client = new HttpClient();
        var siteKey = "BURAYA_SITE_KEY";
        var payload = new { token = siteKey };
        var json = System.Text.Json.JsonSerializer.Serialize(payload);
        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        var response = await client.PostAsync("http://localhost:5239/api/captcha/verify", content);
        var result = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        return result.GetProperty("success").GetBoolean();
    }
    catch
    {
        return true; // API erişilemezse geç
    }
}

    private void OnLoginSuccess(Staff staff)
    {
        LoginSuccessful?.Invoke(staff);
    }

    private void OnCustomerLoginSuccess(Customer customer) => CustomerLoginSuccessful?.Invoke(customer);
}