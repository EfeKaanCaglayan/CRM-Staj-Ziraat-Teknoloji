using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class AdminViewModel : ObservableObject
{
    private readonly AdminRepository _repository = new AdminRepository();
    public string WelcomeMessage => "Hoş geldiniz Admin";

    [ObservableProperty] private List<OrgUnit> _units = new();
    [ObservableProperty] private OrgUnit? _selectedUnit;
    [ObservableProperty] private string _newUnitName = string.Empty;
    [ObservableProperty] private string _newUnitType = string.Empty;

    [ObservableProperty] private List<Staff> _staffList = new();
    [ObservableProperty] private Staff? _selectedStaff;
    [ObservableProperty] private string _newFullName = string.Empty;
    [ObservableProperty] private string _newUsername = string.Empty;
    [ObservableProperty] private string _newPasswordHash = string.Empty;
    [ObservableProperty] private int _newRoleId = 1;

    [ObservableProperty] private List<ActivityLog> _activityLogs = new();

    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private OrgUnit? _selectedUnitForStaff;

    public List<string> UnitTypes { get; } = new() { "Unit", "Branch", "Department", "Region" };
    public List<int> RoleIds { get; } = new() { 1, 2, 3, 4 };

    public AdminViewModel()
    {
        LoadUnits();
        LoadStaff();
        LoadActivityLog();
    }

    private void LoadUnits()
    {
        try
        {
            Units = _repository.GetAllUnits();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    private void LoadStaff()
    {
        try
        {
            StaffList = _repository.GetAllStaff();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    private void LoadActivityLog()
    {
        try
        {
            ActivityLogs = _repository.GetActivityLog();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void AssignStaffToUnit()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedStaff == null)
            {
                ErrorMessage = "Lütfen bir personel seçin.";
                return;
            }
            if (SelectedUnitForStaff == null)
            {
                ErrorMessage = "Lütfen bir birim seçin.";
                return;
            }

            _repository.AssignStaffToUnit(SelectedStaff.StaffId, SelectedUnitForStaff.UnitId, 1);
            SuccessMessage = $"{SelectedStaff.FullName} personeli {SelectedUnitForStaff.UnitName} birimine atandı.";
            SelectedUnitForStaff = null;
            LoadStaff();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CreateUnit()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NewUnitName))
            {
                ErrorMessage = "Birim adı boş bırakılamaz.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewUnitType))
            {
                ErrorMessage = "Birim tipi boş bırakılamaz.";
                return;
            }

            var unitId = _repository.CreateUnit(NewUnitName, NewUnitType, null);
            SuccessMessage = $"Birim #{unitId} oluşturuldu.";
            NewUnitName = string.Empty;
            NewUnitType = string.Empty;
            LoadUnits();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void DeactivateUnit()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedUnit == null)
            {
                ErrorMessage = "Lütfen bir birim seçin.";
                return;
            }

            _repository.DeactivateUnit(SelectedUnit.UnitId);
            SuccessMessage = $"{SelectedUnit.UnitName} birimi pasife alındı.";
            LoadUnits();
            SelectedUnit = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CreateStaff()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(NewFullName))
            {
                ErrorMessage = "Ad soyad boş bırakılamaz.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewUsername))
            {
                ErrorMessage = "Kullanıcı adı boş bırakılamaz.";
                return;
            }
            if (string.IsNullOrWhiteSpace(NewPasswordHash))
            {
                ErrorMessage = "Şifre boş bırakılamaz.";
                return;
            }

            var staffId = _repository.CreateStaff(NewRoleId, NewFullName, NewUsername, NewPasswordHash);
            SuccessMessage = $"Personel #{staffId} oluşturuldu.";
            NewFullName = string.Empty;
            NewUsername = string.Empty;
            NewPasswordHash = string.Empty;
            NewRoleId = 1;
            LoadStaff();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void UpdateStaffRole()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedStaff == null)
            {
                ErrorMessage = "Lütfen bir personel seçin.";
                return;
            }

            _repository.UpdateStaffRole(SelectedStaff.StaffId, NewRoleId);
            SuccessMessage = $"{SelectedStaff.FullName} personelinin rolü güncellendi.";
            LoadStaff();
            SelectedStaff = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
}