using Microsoft.AspNetCore.Mvc;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MarketDataController : ControllerBase
{
    private readonly MarketDataService _market;

    public MarketDataController(MarketDataService market)
    {
        _market = market;
    }

    [HttpGet("{ticker}")]
    public async Task<IActionResult> Get(string ticker)
    {
        var data = await _market.GetLiveMarketData(ticker);

        if (data == null)
            return NotFound();

        return Ok(data);
    }
}
