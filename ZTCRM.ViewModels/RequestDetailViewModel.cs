using System.Collections.Generic;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public class RequestDetailViewModel
{
    public ServiceRequest Request { get; }
    public List<ServiceRequest> PastRequests { get; }

    public RequestDetailViewModel(ServiceRequest request)
    {
        Request = request;

        var repo = new ServiceRequestRepository();
        var all = repo.GetAllByCustomer(request.CustomerId);

       
        PastRequests = all.FindAll(r => r.RequestId != request.RequestId);
    }
}