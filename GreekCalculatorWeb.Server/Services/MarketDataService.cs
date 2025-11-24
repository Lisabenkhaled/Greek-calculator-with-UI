using PricingEngine.Data;
using Shared.DTO;

namespace Server.Services;

public class MarketDataService
{
    private readonly IConfiguration _config;
    private readonly DataFetcher _fetcher;

    public MarketDataService(IConfiguration config)
    {
        _config = config;
        string apiKey = config["AlphaVantage:ApiKey"];
        _fetcher = new DataFetcher(apiKey);
    }

    public async Task<MarketDataResponse?> GetLiveMarketData(string ticker)
    {
        double? spot = await _fetcher.GetSpotAsync(ticker);
        double? rate = await _fetcher.GetRiskFreeRateAsync();

        if (spot == null || rate == null)
            return null;

        return new MarketDataResponse
        {
            Ticker = ticker,
            Spot = spot.Value,
            Rate = rate.Value,
            Volume = null,   // AlphaVantage VOLUME n’est pas standardisé dans GLOBAL_QUOTE
            PERatio = null,
            Timestamp = DateTime.Now
        };
    }
}
