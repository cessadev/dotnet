using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetAll();
    Task<Vehicle?> GetByIdentifier(string identifier);
    Task<bool> HasLoans(string identifier);
    Task Add(Vehicle vehicle);
    Task Remove(Vehicle vehicle);
    Task SaveChanges();
}