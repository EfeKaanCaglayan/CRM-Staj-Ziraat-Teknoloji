using ZTCRM.Data;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// Webhook: N8n'den gelen şikayet oluşturma isteği
app.MapPost("/api/requests/create", async (CreateRequestDto dto) =>
{
    try
    {
        var repo = new ServiceRequestRepository();
        var requestId = repo.Create(dto.CustomerId, dto.RequestType, dto.Description, null);
        return Results.Ok(new { success = true, requestId });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

// Müşteri TC/Pasaport ile arama
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

app.Run();

public record CreateRequestDto(int CustomerId, string RequestType, string Description);