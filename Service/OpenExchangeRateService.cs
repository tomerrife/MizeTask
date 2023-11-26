using Chain.Domain;
using Chain.Interfaces;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chain.Service
{
    public class OpenExchangeRateService<T> : IOpenExchangeRateService<T>, IStorageProvider<T> where T : class
    {
        private readonly HttpClient HttpClient;
        private readonly string ApiKey;

        public OpenExchangeRateService(string apiKey)
        {
            HttpClient = new HttpClient();
            HttpClient.BaseAddress = new System.Uri("https://openexchangerates.org/api/");
            ApiKey = apiKey;
        }
        public async Task<T> GetLatestExchangeRates()
        {
            var apiUrl = $"latest.json?app_id={ApiKey}";
            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            var response = await HttpClient.SendAsync(request);
            if(response.IsSuccessStatusCode)
            {
                var result = await JsonSerializer.DeserializeAsync<T>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                return result;
            }
            return default(T);
        }

        public async Task<T> Read()
        {
            return await GetLatestExchangeRates();
        }
    }
}
