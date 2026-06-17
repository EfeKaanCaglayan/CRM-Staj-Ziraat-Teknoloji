using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly StaffRepository _staffRepository = new StaffRepository();

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

    [RelayCommand]
    private void SelectPersonel()
    {
        IsPersonelTab = true;
        IsMusteriTab = false;
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void SelectMusteri()
    {
        IsPersonelTab = false;
        IsMusteriTab = true;
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Kullanıcı adı veya şifre boş bırakılamaz.";
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

    private void OnLoginSuccess(Staff staff)
    {
        // Sonraki adımda dolduracağız
    }
}