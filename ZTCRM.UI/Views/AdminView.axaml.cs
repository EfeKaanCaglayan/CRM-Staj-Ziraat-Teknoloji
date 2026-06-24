using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class AdminView : Window
{
    private bool _unitsAsc = true;
    private bool _staffAsc = true;
    private bool _activityAsc = true;
    private string _lastUnitsColumn = "";
    private string _lastStaffColumn = "";
    private string _lastActivityColumn = "";

    public AdminView()
    {
        InitializeComponent();
        DataContext = new AdminViewModel();
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        var login = new LoginView();
        login.Show();
        this.Close();
    }

    private void UnitsGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not AdminViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastUnitsColumn == header) _unitsAsc = !_unitsAsc;
        else { _unitsAsc = true; _lastUnitsColumn = header; }

        vm.Units = (header switch
        {
            "ID"        => _unitsAsc ? vm.Units.OrderBy(u => u.UnitId) : vm.Units.OrderByDescending(u => u.UnitId),
            "Birim Adı" => _unitsAsc ? vm.Units.OrderBy(u => u.UnitName) : vm.Units.OrderByDescending(u => u.UnitName),
            "Tip"       => _unitsAsc ? vm.Units.OrderBy(u => u.UnitType) : vm.Units.OrderByDescending(u => u.UnitType),
            "Aktif"     => _unitsAsc ? vm.Units.OrderBy(u => u.IsActiveText) : vm.Units.OrderByDescending(u => u.IsActiveText),
            _           => vm.Units.OrderBy(u => u.UnitId)
        }).ToList();

        e.Handled = true;
    }

    private void StaffGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not AdminViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastStaffColumn == header) _staffAsc = !_staffAsc;
        else { _staffAsc = true; _lastStaffColumn = header; }

        vm.StaffList = (header switch
        {
            "ID"            => _staffAsc ? vm.StaffList.OrderBy(s => s.StaffId) : vm.StaffList.OrderByDescending(s => s.StaffId),
            "Ad Soyad"      => _staffAsc ? vm.StaffList.OrderBy(s => s.FullName) : vm.StaffList.OrderByDescending(s => s.FullName),
            "Kullanıcı Adı" => _staffAsc ? vm.StaffList.OrderBy(s => s.Username) : vm.StaffList.OrderByDescending(s => s.Username),
            "Rol"           => _staffAsc ? vm.StaffList.OrderBy(s => s.RoleId) : vm.StaffList.OrderByDescending(s => s.RoleId),
            "Birim"         => _staffAsc ? vm.StaffList.OrderBy(s => s.UnitName) : vm.StaffList.OrderByDescending(s => s.UnitName),
            "Aktif"         => _staffAsc ? vm.StaffList.OrderBy(s => s.IsActiveText) : vm.StaffList.OrderByDescending(s => s.IsActiveText),
            _               => vm.StaffList.OrderBy(s => s.StaffId)
        }).ToList();

        e.Handled = true;
    }

    private void ActivityGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not AdminViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastActivityColumn == header) _activityAsc = !_activityAsc;
        else { _activityAsc = true; _lastActivityColumn = header; }

        vm.ActivityLogs = (header switch
        {
            "Log ID"     => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.LogId) : vm.ActivityLogs.OrderByDescending(a => a.LogId),
            "Başvuru No" => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.RequestId) : vm.ActivityLogs.OrderByDescending(a => a.RequestId),
            "Eski Durum" => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.OldStatus) : vm.ActivityLogs.OrderByDescending(a => a.OldStatus),
            "Yeni Durum" => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.NewStatus) : vm.ActivityLogs.OrderByDescending(a => a.NewStatus),
            "Değiştiren" => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.ChangedBy) : vm.ActivityLogs.OrderByDescending(a => a.ChangedBy),
            "Tarih"      => _activityAsc ? vm.ActivityLogs.OrderBy(a => a.ChangedAt) : vm.ActivityLogs.OrderByDescending(a => a.ChangedAt),
            _            => vm.ActivityLogs.OrderBy(a => a.LogId)
        }).ToList();

        e.Handled = true;
    }
}