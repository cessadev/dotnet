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
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
    {
        VehicleResponse vehicle = await _vehicleService.Create(request);
        return CreatedAtAction(nameof(GetByIdentifier), new { identifier = vehicle.Identifier }, vehicle);
    }
}