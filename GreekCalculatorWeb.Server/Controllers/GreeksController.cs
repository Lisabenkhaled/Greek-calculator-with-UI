using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.DTO;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GreeksController : ControllerBase
{
    private readonly GreeksService _greeks;

    public GreeksController(GreeksService greeks)
    {
        _greeks = greeks;
    }

    [HttpPost]
    public ActionResult<GreeksResponse> Compute(GreeksRequest req)
    {
        var result = _greeks.ComputeGreeks(req);
        return Ok(result);
    }
}
