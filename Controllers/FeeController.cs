using Microsoft.AspNetCore.Mvc;
using CarCredit.Interfaces;
using CarCredit.Models;

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

    [HttpGet("{creditId}/summary")]
    public async Task<IActionResult> GetSummary(int creditId) => Ok(await _feeService.GetSummary(creditId));

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue() => Ok(await _feeService.GetOverdue());

    [HttpPatch("{feeId}/pay")]
    public async Task<IActionResult> RegisterPayment(
        int feeId, 
        [FromBody] RegisterPaymentRequest request)
    {
        var fee = await _feeService.RegisterPayment(feeId, request.Amount);
        return fee is null ? NotFound() : Ok(fee);
    }
}
