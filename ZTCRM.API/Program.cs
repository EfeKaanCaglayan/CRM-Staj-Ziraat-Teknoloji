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
app.MapPost("/api/captcha/verify", async (CaptchaRequest dto) =>
{
    try
    {
        var secretKey = Environment.GetEnvironmentVariable("RECAPTCHA_SECRET_KEY") 
                        ?? builder.Configuration["RecaptchaSecretKey"];
        
        using var client = new HttpClient();
        var response = await client.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={dto.Token}",
            null);
        
        var json = await response.Content.ReadFromJsonAsync<RecaptchaResponse>();
        
        if (json?.Success == true && json.Score >= 0.5)
            return Results.Ok(new { success = true, score = json.Score });
        
        return Results.Ok(new { success = false, score = json?.Score ?? 0 });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

public record CreateRequestDto(int CustomerId, string RequestType, string Description, string Channel);
public record StatusChangeDto(int RequestId, string Status);
public record CaptchaRequest(string Token);
public record RecaptchaResponse(bool Success, float Score, [property: System.Text.Json.Serialization.JsonPropertyName("error-codes")] string[]? ErrorCodes);
