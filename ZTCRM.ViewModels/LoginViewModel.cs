using System;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ZTCRM.Data;
using ZTCRM.Models;
using System.Linq;
namespace ZTCRM.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly StaffRepository _staffRepository = new StaffRepository();
    private readonly CustomerRepository _customerRepository = new CustomerRepository();
    public event Action<string>? LoginSuccessful;
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
    private void Login()
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

    private void OnLoginSuccess(Staff staff)
    {
        LoginSuccessful?.Invoke(staff.RoleName);
    }

    private void OnCustomerLoginSuccess(Customer customer)
    {
        LoginSuccessful?.Invoke("Customer");
    }
}