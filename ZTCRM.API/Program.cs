using ZTCRM.Data;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapPost("/api/requests/create", async (CreateRequestDto dto) =>
{
    try
    {
        var repo = new ServiceRequestRepository();
        var requestId = repo.Create(dto.CustomerId, dto.RequestType, dto.Description, null, dto.Channel);
        return Results.Ok(new { success = true, requestId });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.MapGet("/api/customers/find", (string nationalId) =>
{
    try
    {
        var repo = new CustomerRepository();
        var customer = repo.Login(nationalId);
        if (customer == null)
            return Results.NotFound(new { success = false, message = "Müşteri bulunamadı" });
        return Results.Ok(new { success = true, customerId = customer.CustomerId, fullName = customer.FullName });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.MapPost("/api/notify/status-change", async (StatusChangeDto dto) =>
{
    try
    {
        var repo = new ServiceRequestRepository();
        var info = repo.GetNotificationInfo(dto.RequestId);
        if (info == null)
            return Results.NotFound(new { success = false, message = "Başvuru bulunamadı" });

        return Results.Ok(new
        {
            success = true,
            requestId = dto.RequestId,
            status = dto.Status,
            channel = info.Channel,
            email = info.Email,
            phone = info.Phone,
            fullName = info.FullName,
            rejectionReason = info.RejectionReason,
            approvalNote = info.ApprovalNote
        });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.Run();

public record CreateRequestDto(int CustomerId, string RequestType, string Description, string Channel);
public record StatusChangeDto(int RequestId, string Status);
