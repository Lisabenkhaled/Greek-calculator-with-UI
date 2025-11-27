using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Globalization;

namespace PricingEngine.Data
{
    public class DataFetcher
    {
        private readonly string _apiKey;
        private readonly HttpClient _http = new HttpClient();

        public DataFetcher(string apiKey)
        {
            _apiKey = apiKey;
        }

        public async Task<double?> GetSpotAsync(string ticker)
        {
            string url =
                $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={ticker}&apikey={_apiKey}";

            var response = await _http.GetStringAsync(url);

            using var json = JsonDocument.Parse(response);

            if (!json.RootElement.TryGetProperty("Global Quote", out var quote))
                return null;

            if (!quote.TryGetProperty("05. price", out var priceElement))
                return null;

            string strPrice = priceElement.GetString();

            return double.Parse(strPrice, CultureInfo.InvariantCulture);
        }

        public async Task<double?> GetRiskFreeRateAsync()
        {
            string url =
                $"https://www.alphavantage.co/query?function=TREASURY_YIELD&interval=daily&apikey={_apiKey}&maturity=3month";

            var response = await _http.GetStringAsync(url);

            using var json = JsonDocument.Parse(response);

            if (!json.RootElement.TryGetProperty("data", out var dataArray))
                return null;

            var latest = dataArray[0];

            string strValue = latest.GetProperty("value").GetString();

            double ratePercent = double.Parse(strValue, CultureInfo.InvariantCulture);

            return ratePercent / 100.0;
        }
    }
}
