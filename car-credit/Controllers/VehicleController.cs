using Microsoft.AspNetCore.Mvc;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;

namespace CarCredit.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<VehicleResponse>>> GetAll() => Ok(await _vehicleService.GetAll());

    [HttpGet("{identifier}")]
    public async Task<ActionResult<VehicleResponse>> GetByIdentifier(string identifier)
    {
        VehicleResponse? vehicle = await _vehicleService.GetByIdentifier(identifier);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterVehicleRequest request)
    {
        VehicleResponse vehicle = await _vehicleService.Create(request);
        return CreatedAtAction(nameof(GetByIdentifier), new { identifier = vehicle.Identifier }, vehicle);
    }

    [HttpPut("{identifier}")]
    public async Task<ActionResult<VehicleResponse>> Update(string identifier, [FromBody] UpdateVehicleRequest request)
    {
        VehicleResponse? vehicle = await _vehicleService.Update(identifier, request);
        return vehicle is null ? NotFound() : Ok(vehicle);
    }

    /// <summary>
    /// Check whether a vehicle is eligible to be financed with a new loan.
    /// </summary>
    /// <param name="identifier">Identifier/plate of the vehicle.</param>
    [HttpGet("{identifier}/eligibility")]
    public async Task<ActionResult<VehicleEligibilityResponse>> CheckEligibility(string identifier)
    {
        VehicleEligibilityResponse? eligibility = await _vehicleService.CheckEligibility(identifier);
        return eligibility is null ? NotFound() : Ok(eligibility);
    }
}