using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ZTCRM.Data;
using ZTCRM.Models;
using Avalonia.Threading;


namespace ZTCRM.ViewModels;

public partial class StaffViewModel : ObservableObject
{
    private readonly StaffRequestRepository _repository = new();
    private readonly Staff _staff;
    private readonly DispatcherTimer _refreshTimer;

    public string WelcomeMessage => $"Hoş geldiniz, {_staff.FullName} | {_staff.UnitName ?? "Birim atanmadı"}";
    private readonly NotificationRepository _notificationRepository = new();
     public bool HasUnread => UnreadCount > 0;

    [ObservableProperty] private List<ServiceRequest> _poolRequests = new();
    [ObservableProperty] private List<ServiceRequest> _myRequests = new();
    [ObservableProperty] private ServiceRequest? _selectedPoolRequest;
    [ObservableProperty] private ServiceRequest? _selectedMyRequest;
    [ObservableProperty] private string _resolutionNote = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;
    [ObservableProperty] private List<Notification> _staffNotifications = new();
    [ObservableProperty] private int _unreadCount;
    [ObservableProperty] private bool _isRefreshing = false;
    [ObservableProperty] private string _poolSearchText = string.Empty;
    [ObservableProperty] private string _myRequestsSearchText = string.Empty;
    [ObservableProperty] private bool _showClosedMyRequests = false;
    [ObservableProperty] private ObservableCollection<ServiceRequest> _filteredPoolRequests = new();
    [ObservableProperty] private ObservableCollection<ServiceRequest> _filteredMyRequests = new();
  
    partial void OnPoolSearchTextChanged(string value) => ApplyPoolFilter();
    partial void OnPoolRequestsChanged(List<ServiceRequest> value) => ApplyPoolFilter();
    partial void OnMyRequestsSearchTextChanged(string value) => ApplyMyRequestsFilter();
    partial void OnMyRequestsChanged(List<ServiceRequest> value) => ApplyMyRequestsFilter();
    partial void OnShowClosedMyRequestsChanged(bool value) => ApplyMyRequestsFilter();

    private void ApplyPoolFilter()
    {
        var filtered = PoolRequests.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(PoolSearchText))
         filtered=filtered.Where(r=>
                     r.CustomerName?.Contains(PoolSearchText,StringComparison.OrdinalIgnoreCase) == true||
                 r.Description?.Contains(PoolSearchText,StringComparison.OrdinalIgnoreCase) == true ||
        r.CategoryName?.Contains(PoolSearchText, StringComparison.OrdinalIgnoreCase) == true||
        r.RequestId.ToString().Contains(PoolSearchText));
        FilteredPoolRequests = new ObservableCollection<ServiceRequest>(filtered);
    }

    private void ApplyMyRequestsFilter()
    {
        var filtered = MyRequests.AsEnumerable();
        if (!ShowClosedMyRequests)
         filtered=filtered.Where(r=>
            r.CurrentStatus !="Kapatıldı");
        if (!string.IsNullOrWhiteSpace(MyRequestsSearchText))
            filtered = filtered.Where(r => 
                r.CustomerName?.Contains(MyRequestsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.Description?.Contains(MyRequestsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.CategoryName?.Contains(MyRequestsSearchText, StringComparison.OrdinalIgnoreCase) == true ||
                r.RequestId.ToString().Contains(MyRequestsSearchText) );
    
        FilteredMyRequests = new ObservableCollection<ServiceRequest>(filtered);
    
            
            
            
        
    }
    
    private void LoadNotifications()
    {
        StaffNotifications = _notificationRepository.GetByStaff(_staff.StaffId);
        UnreadCount = _notificationRepository.GetUnreadCountByStaff(_staff.StaffId);
        OnPropertyChanged(nameof(HasUnread));
    }

    public void MarkNotificationsAsRead()
    {
        _notificationRepository.MarkAsReadByStaff(_staff.StaffId);
        LoadNotifications();
    }

    public StaffViewModel(Staff staff)
    {
        _staff = staff;
        LoadPool();
        LoadMyRequests();
        LoadNotifications();
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
        await Task.Delay(1000);
        LoadPool();
        LoadMyRequests();
        LoadNotifications();
        IsRefreshing = false;
    }

    [RelayCommand]
    private void Refresh()
    {
        RefreshAll();
    }

    private void LoadPool()
    {
        try
        {
            PoolRequests = _repository.GetPool(_staff.StaffId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    private void LoadMyRequests()
    {
        try
        {
            MyRequests = _repository.GetMyRequests(_staff.StaffId);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void AssignToSelf()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedPoolRequest == null)
            {
                ErrorMessage = "Lütfen havuzdan bir başvuru seçin.";
                return;
            }

            _repository.AssignToSelf(SelectedPoolRequest.RequestId, _staff.StaffId);
            SuccessMessage = $"#{SelectedPoolRequest.RequestId} numaralı başvuru üzerinize alındı.";
            LoadPool();
            LoadMyRequests();
            SelectedPoolRequest = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Resolve()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.UpdateStatus(SelectedMyRequest.RequestId, "Resolved", _staff.StaffId, ResolutionNote);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru çözüldü olarak işaretlendi.";
            LoadMyRequests();
            SelectedMyRequest = null;
            ResolutionNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void CannotResolve()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ResolutionNote))
            {
                ErrorMessage = "Çözülemez durumunda açıklama zorunludur.";
                return;
            }

            _repository.UpdateStatus(SelectedMyRequest.RequestId, "CannotResolve", _staff.StaffId, ResolutionNote);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru çözülemez olarak işaretlendi.";
            LoadMyRequests();
            SelectedMyRequest = null;
            ResolutionNote = string.Empty;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ReturnToPool()
    {
        try
        {
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            if (SelectedMyRequest == null)
            {
                ErrorMessage = "Lütfen bir başvuru seçin.";
                return;
            }

            _repository.ReturnToPool(SelectedMyRequest.RequestId, _staff.StaffId);
            SuccessMessage = $"#{SelectedMyRequest.RequestId} numaralı başvuru havuza geri gönderildi.";
            LoadPool();
            LoadMyRequests();
            SelectedMyRequest = null;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Hata: {ex.Message}";
        }
    }
}