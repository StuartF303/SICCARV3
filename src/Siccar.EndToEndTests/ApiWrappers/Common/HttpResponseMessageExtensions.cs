using System.Text.Json;

namespace Siccar.EndToEndTests
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task<T?> GetContent<T>(this HttpResponseMessage httpResponseMessage)
        {
            httpResponseMessage.EnsureSuccessStatusCode();
            var jsonString = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions{PropertyNameCaseInsensitive = true});
        }
    }
}