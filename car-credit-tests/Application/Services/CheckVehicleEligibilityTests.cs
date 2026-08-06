using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

namespace CarCreditTests.Application.Services;

public class CheckVehicleEligibilityTests
{
    // [Vehicle does not exist]
    [Fact]
    public async Task CheckEligibility_VehicleDoesNotExist_ReturnsNull()
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
        VehicleEligibilityResponse? result = await service.CheckEligibility(identifier);

        // Assert
        Assert.Null(result);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(identifier),
            Times.Once
        );

        mockVehicleRepository.Verify(
            r => r.GetActiveLoanReference(It.IsAny<string>()),
            Times.Never
        );
    }

    // [Vehicle has no active loan]
    [Fact]
    public async Task CheckEligibility_NoActiveLoan_ReturnsEligibleTrue()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        Vehicle existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = "MK-1299",
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(existingVehicle.Identifier))
            .ReturnsAsync(existingVehicle);

        mockVehicleRepository
            .Setup(r => r.GetActiveLoanReference(existingVehicle.Identifier))
            .ReturnsAsync((string?)null);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        // Act
        VehicleEligibilityResponse? result = await service.CheckEligibility(existingVehicle.Identifier);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingVehicle.Identifier, result!.VehicleIdentifier);
        Assert.True(result.IsEligible);
        Assert.Null(result.ActiveLoanReference);
        Assert.NotNull(result.Reason);

        mockVehicleRepository.Verify(
            r => r.GetActiveLoanReference(existingVehicle.Identifier),
            Times.Once
        );
    }

    // [Vehicle already has an active loan]
    [Fact]
    public async Task CheckEligibility_HasActiveLoan_ReturnsEligibleFalseWithReason()
    {
        // Arrange
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        Vehicle existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = "MK-1299",
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        const string activeLoanReference = "LN-ABC1234567";

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(existingVehicle.Identifier))
            .ReturnsAsync(existingVehicle);

        mockVehicleRepository
            .Setup(r => r.GetActiveLoanReference(existingVehicle.Identifier))
            .ReturnsAsync(activeLoanReference);

        VehicleService service = new VehicleService(
            mockVehicleRepository.Object
        );

        // Act
        VehicleEligibilityResponse? result = await service.CheckEligibility(existingVehicle.Identifier);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingVehicle.Identifier, result!.VehicleIdentifier);
        Assert.False(result.IsEligible);
        Assert.Equal(activeLoanReference, result.ActiveLoanReference);
        Assert.NotNull(result.Reason);

        mockVehicleRepository.Verify(
            r => r.GetActiveLoanReference(existingVehicle.Identifier),
            Times.Once
        );
    }
}