using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll() => Ok(await _customerService.GetAll());

    [HttpGet("{documentNumber}")]
    public async Task<ActionResult<CustomerResponse>> GetByDocumentNumber(int documentNumber)
    {
        CustomerResponse? customer = await _customerService.GetByDocumentNumber(documentNumber);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        CustomerResponse customer = await _customerService.Create(request);
        return CreatedAtAction(nameof(GetByDocumentNumber), new { documentNumber = customer.DocumentNumber }, customer);
    }

    [HttpDelete("{documentNumber}")]
    public async Task<IActionResult> Delete(int documentNumber)
    {
        try
        {
            bool deleted = await _customerService.Delete(documentNumber);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }
}