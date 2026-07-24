using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

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
            return Conflict(ex.Message);
        }
    }
}