using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace PricingEngine.Data
{
    public class DataCache
    {
        private const string CacheFile = "market_cache.json";

        public double Spot { get; set; }
        public double RiskFreeRate { get; set; }
        public DateTime Timestamp { get; set; }

        public bool IsExpired => (DateTime.Now - Timestamp).TotalHours > 1;

        public static bool Exists => File.Exists(CacheFile);

        public static DataCache Load()
        {
            string json = File.ReadAllText(CacheFile);
            return JsonSerializer.Deserialize<DataCache>(json);
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(CacheFile, json);
        }
    }
}
