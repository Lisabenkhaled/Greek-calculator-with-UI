using System;

namespace Shared.DTO
{
    public class MarketDataResponse
    {
        public string Ticker { get; set; }
        public double Spot { get; set; }
        public double Rate { get; set; }
        public double? Volume { get; set; }
        public double? PERatio { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
