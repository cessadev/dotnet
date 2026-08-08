using Microsoft.EntityFrameworkCore;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly AppDbContext _db;

    public VehicleRepository(AppDbContext _database)
    {
        _db = _database;
    }

    public async Task<IEnumerable<Vehicle>> GetAll() => await _db.Vehicles.ToListAsync();

    public async Task<Vehicle?> GetByIdentifier(string identifier)
        => await _db.Vehicles.FirstOrDefaultAsync(v => v.Identifier == identifier);

    public async Task<bool> HasLoans(string identifier)
        => await _db.Loans.AnyAsync(l => l.Vehicle.Identifier == identifier);

    public Task Add(Vehicle vehicle)
    {
        _db.Vehicles.Add(vehicle);
        return Task.CompletedTask;
    }

    public Task Remove(Vehicle vehicle)
    {
        _db.Vehicles.Remove(vehicle);
        return Task.CompletedTask;
    }

    public async Task SaveChanges() => await _db.SaveChangesAsync();
}