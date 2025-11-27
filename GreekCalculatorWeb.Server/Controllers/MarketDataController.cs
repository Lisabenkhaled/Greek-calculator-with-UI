using Microsoft.AspNetCore.Mvc;
using PricingEngine.Data;
using Shared.DTO;

[ApiController]
[Route("api/market")]
public class MarketDataController : ControllerBase
{
    private readonly IConfiguration _config;

    public MarketDataController(IConfiguration config)
    {
        _config = config;
    }

    [HttpGet("{ticker}")]
    public async Task<ActionResult<MarketDataResponse>> Get(string ticker)
    {
        var apiKey = _config["AlphaVantage:ApiKey"];
        var fetcher = new DataFetcher(apiKey);

        var spot = await fetcher.GetSpotAsync(ticker);

        if (spot is null)
            return NotFound("Ticker introuvable.");

        var rate = await fetcher.GetRiskFreeRateAsync() ?? 0;

        return Ok(new MarketDataResponse
        {
            Ticker = ticker.ToUpper(),
            Spot = spot.Value,
            Rate = rate,
            Timestamp = DateTime.Now
        });
    }
}