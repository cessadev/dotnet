using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.Interfaces;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>
    /// Get the payment history of a specific installment.
    /// </summary>
    /// <param name="paymentReference">Payment reference of the installment.</param>
    [HttpGet("installment/{paymentReference}")]
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetByInstallment(string paymentReference)
        => Ok(await _paymentService.GetByInstallmentReference(paymentReference));

    /// <summary>
    /// Get the full payment history of a loan, across all its installments.
    /// </summary>
    /// <param name="loanReference">Reference of the loan.</param>
    [HttpGet("loan/{loanReference}")]
    public async Task<ActionResult<IEnumerable<PaymentResponse>>> GetByLoan(string loanReference)
        => Ok(await _paymentService.GetByLoanReference(loanReference));
}