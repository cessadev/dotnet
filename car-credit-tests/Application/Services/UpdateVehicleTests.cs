using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class UpdateVehicleTests
{
    // [Vehicle does not exist]
    [Fact]
    public async Task Update_VehicleDoesNotExist_ReturnsNull()
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

        UpdateVehicleRequest request = new UpdateVehicleRequest(
            Brand: EVehicleBrand.Toyota,
            Model: "Passenger Transportation",
            MarketValue: 55_000_000m,
            Year: 2025
        );

        // Act
        VehicleResponse? result = await service.Update(identifier, request);

        // Assert
        Assert.Null(result);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Valid update]
    [Fact]
    public async Task Update_ValidRequest_UpdatesFieldsAndReturnsVehicleResponse()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string identifier = "MK-1299";

        Vehicle existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = identifier,
            Brand = EVehicleBrand.Mazda,
            Model = "Cargo Transportation",
            MarketValue = 50_000_000m,
            Year = 2023
        };

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(identifier))
            .ReturnsAsync(existingVehicle);

        mockVehicleRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        UpdateVehicleRequest request = new UpdateVehicleRequest(
            Brand: EVehicleBrand.Toyota,
            Model: "Passenger Transportation",
            MarketValue: 55_000_000m,
            Year: 2025
        );

        // Act
        VehicleResponse? result = await service.Update(identifier, request);

        // Assert
        Assert.NotNull(result);

        // The Identifier must remain unchanged
        Assert.Equal(identifier, existingVehicle.Identifier);

        Assert.Equal(request.Brand, existingVehicle.Brand);
        Assert.Equal(request.Model, existingVehicle.Model);
        Assert.Equal(request.MarketValue, existingVehicle.MarketValue);
        Assert.Equal(request.Year, existingVehicle.Year);

        Assert.Equal(request.Brand, result!.Brand);
        Assert.Equal(request.Model, result.Model);
        Assert.Equal(request.MarketValue, result.MarketValue);
        Assert.Equal(request.Year, result.Year);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.SaveChanges(),
            Times.Once
        );
    }
}