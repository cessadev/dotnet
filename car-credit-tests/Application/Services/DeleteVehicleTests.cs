using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class DeleteVehicleTests
{
    // [Vehicle not found]
    [Fact]
    public async Task Delete_VehicleDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string identifier = "MK-1299";

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(identifier))
            .ReturnsAsync((Vehicle?)null);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        // Act
        bool result = await service.Delete(identifier);

        // Assert
        Assert.False(result);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.HasLoans(It.IsAny<string>()),
            Times.Never
        );

        mockVehicleRepository.Verify(
            r => r.Remove(It.IsAny<Vehicle>()),
            Times.Never
        );

        mockVehicleRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Vehicle has associated loans]
    [Fact]
    public async Task Delete_VehicleHasLoans_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string identifier = "MK-1299";

        Vehicle existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = identifier,
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(identifier))
            .ReturnsAsync(existingVehicle);

        mockVehicleRepository
            .Setup(r => r.HasLoans(identifier))
            .ReturnsAsync(true);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.Delete(identifier)
        );

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.HasLoans(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.Remove(It.IsAny<Vehicle>()),
            Times.Never
        );

        mockVehicleRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Vehicle has no associated loans]
    [Fact]
    public async Task Delete_VehicleHasNoLoans_DeletesVehicle()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string identifier = "MK-1299";

        Vehicle existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = identifier,
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(identifier))
            .ReturnsAsync(existingVehicle);

        mockVehicleRepository
            .Setup(r => r.HasLoans(identifier))
            .ReturnsAsync(false);

        mockVehicleRepository
            .Setup(r => r.Remove(existingVehicle))
            .Returns(Task.CompletedTask);

        mockVehicleRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        // Act
        bool result = await service.Delete(identifier);

        // Assert
        Assert.True(result);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.HasLoans(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.Remove(existingVehicle),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.SaveChanges(),
            Times.Once
        );
    }
}