using CarCredit.Interfaces;
using CarCredit.Models;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<IActionResult> GetAll() => Ok(await _customerService.GetAll());

    [HttpGet("{customerId}")]
    public async Task<IActionResult> GetById(int customerId)
    {
        var customer = await _customerService.GetById(customerId);
        return customer is null ? NotFound() : Ok(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = await _customerService.Create(request);
        return CreatedAtAction(nameof(GetById), new { customerId = customer.Id }, customer);
    }

    [HttpDelete("{customerId}")]
    public async Task<IActionResult> Delete(int customerId)
    {
        var deleted = await _customerService.Delete(customerId);
        return deleted ? NoContent() : NotFound();
    }
}