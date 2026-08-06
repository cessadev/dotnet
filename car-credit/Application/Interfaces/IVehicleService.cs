using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;

namespace CarCredit.Application.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleResponse>> GetAll();
    Task<VehicleResponse?> GetByIdentifier(string identifier);
    Task<VehicleResponse> Create(RegisterVehicleRequest request);
    Task<VehicleResponse?> Update(string identifier, UpdateVehicleRequest request);
    Task<VehicleEligibilityResponse?> CheckEligibility(string identifier);
}