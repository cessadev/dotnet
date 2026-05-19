using Microsoft.AspNetCore.Mvc;
using CarCredit.Models;
using CarCredit.Interfaces;

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
    public async Task<IActionResult> GetAll() => Ok(await _creditService.GetAll());

    [HttpGet("{creditId}")]
    public async Task<IActionResult> GetById(int creditId)
    {
        var credit = await _creditService.GetById(creditId);
        return credit is null ? NotFound() : Ok(credit);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCreditRequest request)
    {
        try
        {
            var credit = await _creditService.CreateWithFees(request);
            return CreatedAtAction(nameof(GetById), new { creditId = credit.Id }, credit);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int creditId)
    {
        var deleted = await _creditService.Delete(creditId);
        return deleted ? NoContent() : NotFound();
    }

}