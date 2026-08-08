using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class VehicleService : IVehicleService
{
    private readonly IVehicleRepository _vehicleRepository;

    public VehicleService(IVehicleRepository vehicleRepository)
    {
        _vehicleRepository = vehicleRepository;
    }

    public async Task<IEnumerable<VehicleResponse>> GetAll()
    {
        IEnumerable<Vehicle> vehicles = await _vehicleRepository.GetAll();
        return vehicles.Select(ToResponse);
    }

    public async Task<VehicleResponse?> GetByIdentifier(string identifier)
    {
        Vehicle? vehicle = await _vehicleRepository.GetByIdentifier(identifier);
        return vehicle is null ? null : ToResponse(vehicle);
    }

    public async Task<VehicleResponse> Create(RegisterVehicleRequest request)
    {
        Vehicle vehicle = new Vehicle
        {
            Identifier = request.Identifier,
            Brand = request.Brand,
            Model = request.Model,
            MarketValue = request.MarketValue,
            Year = request.Year
        };

        await _vehicleRepository.Add(vehicle);
        await _vehicleRepository.SaveChanges();

        return ToResponse(vehicle);
    }

    public async Task<VehicleResponse?> Update(string identifier, UpdateVehicleRequest request)
    {
        Vehicle? vehicle = await _vehicleRepository.GetByIdentifier(identifier);
        if (vehicle is null) return null;

        vehicle.Brand = request.Brand;
        vehicle.Model = request.Model;
        vehicle.MarketValue = request.MarketValue;
        vehicle.Year = request.Year;

        await _vehicleRepository.SaveChanges();

        return ToResponse(vehicle);
    }

    public async Task<bool> Delete(string identifier)
    {
        Vehicle? vehicle = await _vehicleRepository.GetByIdentifier(identifier);
        if (vehicle is null) return false;

        if(await _vehicleRepository.HasLoans(identifier))
            throw new InvalidOperationException(
                "Este vehículo no puede ser eliminado ya que está asociado a préstamos vigentes.");

        await _vehicleRepository.Remove(vehicle);
        await _vehicleRepository.SaveChanges();

        return true;
    }

    private static VehicleResponse ToResponse(Vehicle v) => new(
        v.Identifier,
        v.Brand,
        v.Model,
        v.MarketValue,
        v.Year
    );
}