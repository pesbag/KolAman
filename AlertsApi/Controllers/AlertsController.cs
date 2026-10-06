using AlertsApi.Repository;
using AlertsApi.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace AlertsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlertsController : ControllerBase
{
    private readonly IAlertRepository _repo;
    public AlertsController(IAlertRepository repo)
    {
        _repo = repo;
    }
    [HttpGet("GetNumberOfTotalAlerts")]
    public async Task<ActionResult<RegionTotalAlertsDto>> GetNuberOfAllAlertsInAllRegionAsync()
    {
        return Ok(await _repo.GetNumberOfAlertsInUnitAsync());
    }
}
