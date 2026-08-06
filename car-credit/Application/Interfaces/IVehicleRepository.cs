using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface IVehicleRepository
{
    Task<IEnumerable<Vehicle>> GetAll();
    Task<Vehicle?> GetByIdentifier(string identifier);
    Task<string?> GetActiveLoanReference(string identifier);
    Task Add(Vehicle vehicle);
    Task SaveChanges();
}