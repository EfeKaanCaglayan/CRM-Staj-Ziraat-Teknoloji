using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
using Avalonia.Threading;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;


namespace ZTCRM.ViewModels;

public partial class AdminViewModel : ObservableObject
{ private readonly DispatcherTimer _refreshTimer;
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
    [ObservableProperty] private string _newRoleName = string.Empty;
    [ObservableProperty] private bool _isRefreshing = false;
    [ObservableProperty] private bool _showOnlyActiveUnits = true;
    [ObservableProperty] private bool _showOnlyActiveStaff = true;
    [ObservableProperty] private string _logSearchText = string.Empty;
    [ObservableProperty] private List<OrgUnit> _filteredUnits = new();
    [ObservableProperty] private List<Staff> _filteredStaffList = new();
    [ObservableProperty] private List<ActivityLog> _filteredActivityLogs = new();
    [ObservableProperty] private string _unitsSearchText = string.Empty;
    [ObservableProperty] private string _staffSearchText = string.Empty;
    [ObservableProperty] private string _updateRoleName = string.Empty;

    partial void OnUnitsSearchTextChanged(string value) => ApplyUnitsFilter();
    partial void OnStaffSearchTextChanged(string value) => ApplyStaffFilter();
    partial void OnShowOnlyActiveUnitsChanged(bool value) => ApplyUnitsFilter();
    partial void OnUnitsChanged(List<OrgUnit> value) => ApplyUnitsFilter();
    partial void OnShowOnlyActiveStaffChanged(bool value) => ApplyStaffFilter();
    partial void OnStaffListChanged(List<Staff> value) => ApplyStaffFilter();
    partial void OnLogSearchTextChanged(string value) => ApplyLogFilter();
    partial void OnActivityLogsChanged(List<ActivityLog> value) => ApplyLogFilter();
    private void ApplyUnitsFilter()
    {
        var filtered = ShowOnlyActiveUnits 
            ? Units.Where(u => u.IsActive).ToList() 
            : Units.ToList();
        if (!string.IsNullOrWhiteSpace(UnitsSearchText))
            filtered = filtered.Where(u =>
                u.UnitId.ToString().Contains(UnitsSearchText) ||
                u.UnitName?.Contains(UnitsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                u.UnitType?.Contains(UnitsSearchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
        FilteredUnits = filtered;
    }

    private void ApplyStaffFilter()
    {
        var filtered = ShowOnlyActiveStaff 
            ? StaffList.Where(s => s.IsActive).ToList() 
            : StaffList.ToList();
        if (!string.IsNullOrWhiteSpace(StaffSearchText))
            filtered = filtered.Where(s =>
                s.StaffId.ToString().Contains(StaffSearchText) ||
                s.FullName?.Contains(StaffSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                s.Username?.Contains(StaffSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                s.RoleName?.Contains(StaffSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                s.UnitName?.Contains(StaffSearchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
        FilteredStaffList = filtered;
    }

    private void ApplyLogFilter()
    {
        if (string.IsNullOrWhiteSpace(LogSearchText))
        {
            FilteredActivityLogs = ActivityLogs;
            return;
        }
        FilteredActivityLogs = ActivityLogs.Where(l =>
            l.RequestId.ToString().Contains(LogSearchText) ||
            l.OldStatus?.Contains(LogSearchText, StringComparison.OrdinalIgnoreCase) == true ||
            l.NewStatus?.Contains(LogSearchText, StringComparison.OrdinalIgnoreCase) == true ||
            l.ChangedBy?.Contains(LogSearchText, StringComparison.OrdinalIgnoreCase) == true).ToList();
    }
    
    
    public List<string> RoleNames { get; } = new() { "Admin", "Operatör", "Personel", "Yönetici" };
    public List<string> UnitTypes { get; } = new() { "Birim", "Şube", "Departman", "Bölge" };    
    public List<int> RoleIds { get; } = new() { 1, 2, 3, 4 };
    
    public AdminViewModel()
    {
        LoadUnits();
        LoadStaff();
        LoadActivityLog();
        _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(30)
        };
        _refreshTimer.Tick += (s, e) => RefreshAll();
        _refreshTimer.Start();
    }
    private async void RefreshAll()
    {
        IsRefreshing = true;
        await Task.Delay(100);
        LoadUnits();
        LoadStaff();
        LoadActivityLog();
        IsRefreshing = false;
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshAll();
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

            var mappedType = NewUnitType switch
            {
                "Birim"     => "Unit",
                "Şube"      => "Branch",
                "Departman" => "Department",
                "Bölge"     => "Region",
                _           => NewUnitType
            };

            var unitId = _repository.CreateUnit(NewUnitName, mappedType, null);
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
        var mappedRole = NewRoleName switch
        {
            "Admin"    => 1,
            "Operatör" => 2,
            "Personel" => 3,
            "Yönetici" => 4,
            _          => 3
        };
      
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

            var staffId = _repository.CreateStaff(mappedRole, NewFullName, NewUsername, NewPasswordHash);   
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
        var mappedRole = UpdateRoleName switch
        {
            "Admin"    => 1,
            "Operatör" => 2,
            "Personel" => 3,
            "Yönetici" => 4,
            _          => 3
        };
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedStaff == null)
            {
                ErrorMessage = "Lütfen bir personel seçin.";
                return;
            }

            _repository.UpdateStaffRole(SelectedStaff.StaffId, mappedRole);
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