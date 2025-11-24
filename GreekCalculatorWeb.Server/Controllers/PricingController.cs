using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.DTO;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PricingController : ControllerBase
{
    private readonly PricingService _pricing;

    public PricingController(PricingService pricing)
    {
        _pricing = pricing;
    }

    [HttpPost]
    public ActionResult<double> Price(PricingRequest req)
    {
        double value = _pricing.ComputePrice(req);
        return Ok(value);
    }
}
