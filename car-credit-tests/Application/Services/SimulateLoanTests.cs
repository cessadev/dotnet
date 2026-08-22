using Moq;
using Xunit;
using CarCredit.Application.Services;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class SimulateLoanTests
{
    // [No vehicle provided]
    [Fact]
    public async Task Simulate_WithoutVehicleIdentifier_SkipsVehicleValidation()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12,
            VehicleIdentifier: null
        );

        // Act
        LoanSimulation result = await service.Simulate(request);

        // Assert
        Assert.Equal(10_000_000m, result.Amount);
        Assert.Equal(EInstallmentsTerm.Months12, result.Installments);
        Assert.Equal(12, result.Schedule.Count());
        Assert.Equal(10_280_000m, result.TotalToPay);

        mockVehicleRepository.Verify(
            r => r.GetByIdentifier(It.IsAny<string>()),
            Times.Never
        );
    }

    // [Vehicle does not exist]
    [Fact]
    public async Task Simulate_VehicleDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier("MK-1299"))
            .ReturnsAsync((Vehicle?)null);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12,
            VehicleIdentifier: "MK-1299"
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.Simulate(request)
        );
    }

    // [Amount exceeds vehicle market value]
    [Fact]
    public async Task Simulate_AmountExceedsVehicleMarketValue_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
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

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 60_000_000m,
            Installments: EInstallmentsTerm.Months12,
            VehicleIdentifier: existingVehicle.Identifier
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.Simulate(request)
        );
    }

    // [Valid request + rounding, with interest applied]
    [Fact]
    public async Task Simulate_ValidRequest_DistributesRoundingRemainderCorrectly()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12,
            VehicleIdentifier: null
        );

        // Act
        LoanSimulation result = await service.Simulate(request);

        // Assert
        List<SimulatedInstallment> schedule = result.Schedule.ToList();

        Assert.Equal(12, schedule.Count);
        Assert.Equal(10_280_000m, schedule.Sum(i => i.Amount));
        Assert.Equal(856_666.66m, result.InstallmentValue);

        Assert.All(
            schedule.Take(11),
            installment => Assert.Equal(856_666.66m, installment.Amount)
        );

        Assert.Equal(856_666.74m, schedule[11].Amount);
        Assert.Equal(10_280_000m, result.TotalToPay);
    }

    // [Interest breakdown is exposed for transparency]
    [Fact]
    public async Task Simulate_ValidRequest_ExposesInterestRateAndAmountSeparately()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12,
            VehicleIdentifier: null
        );

        // Act
        LoanSimulation result = await service.Simulate(request);

        // Assert
        Assert.Equal(0.028m, result.InterestRate);
        Assert.Equal(280_000m, result.InterestAmount);
        Assert.Equal(10_280_000m, result.TotalAmount);
        Assert.Equal(10_000_000m, result.Amount); // principal untouched
    }

    // [Verify that the number of installments is correct]
    [Theory]
    [InlineData(EInstallmentsTerm.Months6, 6)]
    [InlineData(EInstallmentsTerm.Months12, 12)]
    [InlineData(EInstallmentsTerm.Months18, 18)]
    [InlineData(EInstallmentsTerm.Months24, 24)]
    [InlineData(EInstallmentsTerm.Months30, 30)]
    [InlineData(EInstallmentsTerm.Months36, 36)]
    [InlineData(EInstallmentsTerm.Months42, 42)]
    [InlineData(EInstallmentsTerm.Months48, 48)]
    public async Task Simulate_ValidInstallmentTerm_ReturnsExpectedNumberOfInstallments(
        EInstallmentsTerm installmentTerm,
        int expectedInstallments)
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        SimulateLoanRequest request = new SimulateLoanRequest(
            Amount: 10_000_000m,
            Installments: installmentTerm,
            VehicleIdentifier: null
        );

        // Act
        LoanSimulation result = await service.Simulate(request);

        // Assert
        Assert.Equal(expectedInstallments, result.Schedule.Count());
    }
}