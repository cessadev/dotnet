using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;

namespace CarCredit.Application.Interfaces;

public interface IVehicleService
{
    Task<IEnumerable<VehicleResponse>> GetAll();
    Task<VehicleResponse?> GetByIdentifier(string identifier);
    Task<VehicleResponse> Create(CreateVehicleRequest request);
}