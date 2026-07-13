using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CreditController : ControllerBase
{
    private readonly ICreditService _creditService;

    public CreditController(ICreditService creditService)
    {
        _creditService = creditService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CreditResponse>>> GetAllCredits() => Ok(await _creditService.GetAll());

    [HttpGet("{creditId}")]
    public async Task<ActionResult<CreditResponse>> GetById(int creditId)
    {
        CreditResponse credit = await _creditService.GetById(creditId);
        return credit is null ? NotFound() : Ok(credit);
    }

    /// <summary>
    /// Create a new credit and automatically generate its repayment schedule.
    /// </summary>
    /// <param name="request">Details of the loan to be created.</param>
    /// <returns>Credit created.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCreditRequest request)
    {
        try
        {
            CreditResponse credit = await _creditService.Create(request);
            return CreatedAtAction(nameof(GetById), new { creditId = credit.Id }, credit);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message); // 400
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message }); // 404
        }
    }

    [HttpDelete("{creditId}")]
    public async Task<IActionResult> Delete(int creditId)
    {
        try
        {
            bool deleted = await _creditService.Delete(creditId);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}