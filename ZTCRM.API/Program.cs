using ZTCRM.Data;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

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

app.MapPost("/api/groq/validate", async (GroqValidateDto dto) =>
{
    try
    {
        var groq = new GroqService();
        var messages = new List<(string role, string content)>
        {
            ("user", $"Müşteri adı: {dto.FullName}\nŞikayet türü: {dto.RequestType}\nAçıklama: {dto.Description}")
        };
        var response = await groq.SendMessageAsync(messages);
        var isApproved = response.Contains("Bilgiler tam");
        return Results.Ok(new { success = true, message = response, approved = isApproved });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

app.MapPost("/api/groq/suggest-category", async (SuggestCategoryDto dto) =>
{
    try
    {
        var groq = new GroqService();
        var categoryListText = string.Join("\n", dto.Categories);

        var systemPrompt = $@"Sen bir banka şikayet sınıflandırma asistanısın. Sana bir müşteri şikayeti açıklaması verilecek, görevin aşağıdaki listeden EN UYGUN alt kategoriyi seçmek.

Kategori Listesi:
{categoryListText}

KURALLAR:
- Cevabın SADECE listedeki kategori adlarından biri olmalı, birebir aynı yazımla.
- Hiçbir açıklama, ek kelime veya noktalama ekleme.
- Emin olamadığın durumlarda en yakın anlamlı kategoriyi seç.";

        var suggested = await groq.SendRawMessageAsync(systemPrompt, dto.Description);

        return Results.Ok(new { success = true, category = suggested });
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { success = false, error = ex.Message });
    }
});

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

app.Run();

public record CreateRequestDto(int CustomerId, string RequestType, string Description, string Channel);
public record StatusChangeDto(int RequestId, string Status);
public record CaptchaRequest(string Token);
public record RecaptchaResponse(bool Success, float Score, [property: System.Text.Json.Serialization.JsonPropertyName("error-codes")] string[]? ErrorCodes);
public record GroqValidateDto(string FullName, string RequestType, string Description);
public record SuggestCategoryDto(string Description, List<string> Categories);