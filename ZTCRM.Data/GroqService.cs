using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ZTCRM.Data;

public class GroqService
{
    private readonly HttpClient _client = new();
    private readonly string _apiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY") ?? "";
    private const string Url = "https://api.groq.com/openai/v1/chat/completions";

    private const string SystemPrompt = @"Sen bir banka müşteri hizmetleri asistanısın.
Müşteri zaten sistemde kayıtlı, kimliği doğrulanmış, iletişim bilgileri ve hesap bilgileri sistemde mevcut.
Asla iletişim bilgisi, hesap numarası, IBAN, kimlik bilgisi, kart numarasının tamamı isteme.
Sadece şikayetin konusunu anlamak için gereken minimum bilgiyi iste.
Kayıp kart şikayetinde sadece kartın son 4 hanesi ve kaybolma tarihi yeterlidir.

Eğer tüm gerekli bilgiler mevcutsa SADECE şunu yaz: 'Bilgiler tam, başvuruyu gönderebilirsiniz.'
Eğer eksik bilgi varsa SADECE şu formatta yaz:
'Eksik bilgiler:
- [eksik bilgi 1]
- [eksik bilgi 2]'

Başka hiçbir şey yazma. Türkçe konuş.";

    public async Task<string> SendMessageAsync(List<(string role, string content)> messages)
    {
        try
        {
            var messageList = new List<object>
            {
                new { role = "system", content = SystemPrompt }
            };

            foreach (var (role, content) in messages)
            {
                messageList.Add(new { role, content });
            }

            var body = new
            {
                model = "llama-3.3-70b-versatile",
                messages = messageList,
                max_tokens = 500
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Url);
            request.Headers.Add("Authorization", $"Bearer {_apiKey}");
            request.Content = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Groq yanıt: {json}"); // debug
            var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? "";
        }
        catch (Exception ex)
        {
            return $"AI bağlantı hatası: {ex.Message}";
        }
    }}