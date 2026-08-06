using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    /// <summary>
    /// Get aggregated portfolio KPIs: total placed, collected, overdue and delinquency rate.
    /// </summary>
    [HttpGet("summary")]
    public async Task<ActionResult<DashboardSummary>> GetSummary() => Ok(await _dashboardService.GetSummary());
}