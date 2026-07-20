using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InstallmentController : ControllerBase
{
    private readonly IInstallmentService _installmentService;

    public InstallmentController(IInstallmentService installmentService)
    {
        _installmentService = installmentService;
    }

    [HttpGet("loan/{loanReference}")]
    public async Task<ActionResult<IEnumerable<InstallmentResponse>>> GetByLoan(string loanReference)
        => Ok(await _installmentService.GetAllByLoanReference(loanReference));

    [HttpGet("loan/{loanReference}/summary")]
    public async Task<ActionResult<LoanSummary>> GetSummary(string loanReference)
    {
        LoanSummary? summary = await _installmentService.GetSummaryByLoanReference(loanReference);
        return summary is null ? NotFound() : Ok(summary);
    }

    [HttpGet("loan/{loanReference}/overdue")]
    public async Task<ActionResult<IEnumerable<OverdueInstallment>>> GetOverdueByLoan(string loanReference)
        => Ok(await _installmentService.GetOverdueByLoanReference(loanReference));

    [HttpGet("overdue")]
    public async Task<ActionResult<IEnumerable<OverdueInstallment>>> GetAllOverdue()
        => Ok(await _installmentService.GetAllOverdue());

    [HttpPatch("{paymentReference}/pay")]
    public async Task<ActionResult<InstallmentResponse>> RegisterPayment(
        string paymentReference,
        [FromBody] RegisterPaymentRequest request)
    {
        InstallmentResponse? installment = await _installmentService.RegisterPayment(paymentReference, request.Method, request.Amount);
        return installment is null ? NotFound() : Ok(installment);
    }
}