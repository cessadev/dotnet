using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

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
    public async Task<ActionResult<IEnumerable<FeeResponse>>> GetFeesByCredit(int creditId)
    {
        IEnumerable<FeeResponse> fees = await _feeService.GetByCreditId(creditId);
        return Ok(fees);
    }

    [HttpGet("{creditId}/summary")]
    public async Task<ActionResult<CreditSummary>> GetSummaryByCredit(int creditId)
    {
        CreditSummary? summary = await _feeService.GetSummary(creditId);
        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<OverdueFee>>> GetOverdueFees()
    {
        IEnumerable<OverdueFee> overdueFees = await _feeService.GetOverdue();
        return Ok(overdueFees);
    }

    [HttpPatch("{feeId}/pay")]
    public async Task<ActionResult<FeeResponse>> RegisterPayment(
        int feeId, 
        [FromBody] RegisterPaymentRequest request)
    {
        FeeResponse? fee = await _feeService.RegisterPayment(feeId, request.Amount);
        return fee is null ? NotFound() : Ok(fee);
    }
}