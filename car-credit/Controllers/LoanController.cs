using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.Converters;
using CarCredit.Domain.Enums;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanController : ControllerBase
{
    private readonly ILoanService _loanService;

    public LoanController(ILoanService loanService)
    {
        _loanService = loanService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetAll() => Ok(await _loanService.GetAll());

    [HttpGet("{reference}")]
    public async Task<ActionResult<LoanResponse>> GetByReference(string reference)
    {
        LoanResponse? loan = await _loanService.GetByReference(reference);
        return loan is null ? NotFound() : Ok(loan);
    }

    /// <summary>
    /// Get the loans associated with a customer, identified by document type and document number.
    /// </summary>
    [HttpGet("customer")]
    public async Task<ActionResult<IEnumerable<LoanResponse>>> GetByCustomer(
        [FromQuery] CustomerLoansRequest request)
    {
        if (!DocumentTypeCodes.TryParse(request.DocumentType, out EDocumentType documentType))
            return BadRequest(new
            {
                Message = $"'{request.DocumentType}' no es un tipo de documento válido. Se espera: {string.Join(", ", DocumentTypeCodes.FromCode.Keys)}."
            });

        try
        {
            IEnumerable<LoanResponse> loans = await _loanService.GetByCustomer(documentType, request.DocumentNumber);
            return Ok(loans);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    /// <summary>
    /// Create a new credit and automatically generate its repayment schedule.
    /// </summary>
    /// <param name="request">Details of the loan to be created.</param>
    /// <returns>Credit created.</returns>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLoanRequest request)
    {
        try
        {
            LoanResponse loan = await _loanService.Create(request);
            return CreatedAtAction(nameof(GetByReference), new { reference = loan.Reference }, loan);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    /// <summary>
    /// Simulate a credit's repayment schedule without persisting anything.
    /// </summary>
    /// <param name="request">Amount, term and optionally a vehicle to validate against its market value.</param>
    [HttpPost("simulate")]
    public async Task<ActionResult<LoanSimulation>> Simulate([FromBody] SimulateLoanRequest request)
    {
        try
        {
            LoanSimulation simulation = await _loanService.Simulate(request);
            return Ok(simulation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { ex.Message });
        }
    }

    [HttpDelete("{reference}")]
    public async Task<IActionResult> Delete(string reference)
    {
        try
        {
            bool deleted = await _loanService.Delete(reference);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { ex.Message });
        }
    }
}