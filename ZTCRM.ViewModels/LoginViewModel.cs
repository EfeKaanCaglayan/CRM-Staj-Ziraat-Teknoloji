
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly StaffRepository _staffRepository = new StaffRepository();

    [ObservableProperty]
    private string _username=string.Empty;
    
    [ObservableProperty]
    private string _password=string.Empty;
    
    [ObservableProperty]
    private string _errorMessage=string.Empty;

    [RelayCommand]
    private void Login()
    {
        ErrorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(Username)|| string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Kullanıcı adı veya şifre boş bırakılamaz.";
            return;
        }

        var staff = _staffRepository.Login(Username, Password);
        if (staff == null)
        {
            ErrorMessage = "Kullanıcı adı veya şifre hatalı";
            return;
        }

        OnLoginSucsess(staff);
        
      
    }
    private void OnLoginSucsess(Staff staff){}

}