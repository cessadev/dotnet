using Moq;
using Xunit;
using CarCredit.Application.Services;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

namespace CarCreditTests.Application.Services;

public class LoanServiceTests
{
    [Fact]
    public async void Create_ValidRequest_DistributesRoundingRemainderCorrectly()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        Customer existingCustomer = new Customer
        {
            Id = 1,
            DocumentType = EDocumentType.CedulaCiudadania,
            DocumentNumber = 123456789,
            Name = "Michael",
            Lastname = "Olise",
            Age = 25,
            Address = "CL 105E 41T, Paris"
        };

        var existingVehicle = new Vehicle
        {
            Id = 1,
            Identifier = "MK-1299",
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(existingCustomer.DocumentNumber))
            .ReturnsAsync(existingCustomer);

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(existingVehicle.Identifier))
            .ReturnsAsync(existingVehicle);

        IEnumerable<Installment>? capturedInstallments = null;

        mockLoanRepository
            .Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()))
            .Callback<IEnumerable<Installment>>(Installments => capturedInstallments = Installments)
            .Returns(Task.CompletedTask);

        mockLoanRepository.Setup(r => r.Add(It.IsAny<Loan>())).Returns(Task.CompletedTask);
        mockLoanRepository.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        CreateLoanRequest request = new CreateLoanRequest(
            CustomerDocumentNumber: existingCustomer.DocumentNumber,
            VehicleIdentifier: existingVehicle.Identifier,
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12
        );

        // Act
        LoanResponse result = await service.Create(request);

        // Assert
        Assert.NotNull(capturedInstallments);
        var list = capturedInstallments!.ToList();

        Assert.Equal(12, list.Count);
        Assert.Equal(10_000_000m, list.Sum(i => i.Amount));
        
        Assert.All(
            list.Take(11),
            installment => Assert.Equal(833_333.33m, installment.Amount)
        );

        Assert.Equal(833_333.37m, list[11].Amount);
    }
}