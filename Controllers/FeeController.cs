using Microsoft.AspNetCore.Mvc;
using CarCredit.Interfaces;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeeController : ControllerBase
{
    private readonly IFeeService _feeService;

    public FeeController(IFeeService feeService)
    {
        _feeService = feeService;
    }

    [HttpGet("credit/{creditId}")]
    public async Task<IActionResult> GetByCreditId(int creditId) => Ok(await _feeService.GetByCreditId(creditId));

    [HttpGet("summary/{creditId}")]
    public async Task<IActionResult> GetSummary(int creditId) => Ok(await _feeService.GetSummary(creditId));

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue() => Ok(await _feeService.GetOverdue());

    [HttpPatch("{feeId}/pay")]
    public async Task<IActionResult> RegisterPayment(int feeId, [FromBody] decimal valuePaid)
    {
        var fee = await _feeService.RegisterPayment(feeId, valuePaid);
        return fee is null ? NotFound() : Ok(fee);
    }
}