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
    public async Task<IActionResult<IEnumerable<CustomerResponse>>> GetAll() => Ok(await _customerService.GetAll());

    [HttpGet("{customerId}")]
    public async Task<IActionResult<CustomerResponse>> GetById(int customerId)
    {
        CustomerResponse customer = await _customerService.GetById(customerId);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCustomerRequest request)
    {
        CustomerResponse customer = await _customerService.Create(request);
        return CreatedAtAction(nameof(GetById), new { customerId = customer.Id }, customer);
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> Delete(int customerId)
    {
        bool deleted = await _customerService.Delete(customerId);
        return deleted ? NoContent() : NotFound();
    }
}