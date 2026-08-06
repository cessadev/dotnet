using Moq;
using Xunit;
using CarCredit.Application.Services;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class GetLoansByCustomerTests
{
    // [Customer does not exist]
    [Fact]
    public async Task GetByCustomer_CustomerDoesNotExist_ThrowsKeyNotFoundException()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const int documentNumber = 123456789;

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync((Customer?)null);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetByCustomer(EDocumentType.CedulaCiudadania, documentNumber)
        );

        mockLoanRepository.Verify(
            r => r.GetByCustomerDocumentNumber(It.IsAny<int>()),
            Times.Never
        );
    }

    // [DocumentType does not match the customer on file]
    [Fact]
    public async Task GetByCustomer_DocumentTypeMismatch_ThrowsKeyNotFoundException()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const int documentNumber = 123456789;

        Customer existingCustomer = new Customer
        {
            Id = 1,
            DocumentType = EDocumentType.CedulaCiudadania,
            DocumentNumber = documentNumber,
            Name = "Michael",
            Lastname = "Olise",
            Age = 25,
            Address = "CL 105E 41T, Paris"
        };

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync(existingCustomer);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.GetByCustomer(EDocumentType.Pasaporte, documentNumber)
        );

        mockLoanRepository.Verify(
            r => r.GetByCustomerDocumentNumber(It.IsAny<int>()),
            Times.Never
        );
    }

    // [Customer has registered loans]
    [Fact]
    public async Task GetByCustomer_ValidRequest_ReturnsMappedLoanResponses()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const int documentNumber = 123456789;

        Customer existingCustomer = new Customer
        {
            Id = 1,
            DocumentType = EDocumentType.CedulaCiudadania,
            DocumentNumber = documentNumber,
            Name = "Michael",
            Lastname = "Olise",
            Age = 25,
            Address = "CL 105E 41T, Paris"
        };

        Vehicle firstVehicle = new Vehicle
        {
            Id = 1,
            Identifier = "MK-1299",
            Brand = EVehicleBrand.Toyota,
            Model = "Passenger Transportation",
            MarketValue = 50_000_000m,
            Year = 2025
        };

        Vehicle secondVehicle = new Vehicle
        {
            Id = 2,
            Identifier = "MK-4521",
            Brand = EVehicleBrand.Mazda,
            Model = "Cargo Transportation",
            MarketValue = 40_000_000m,
            Year = 2022
        };

        List<Loan> existingLoans = new()
        {
            new Loan
            {
                Id = 1,
                Reference = "LN-AAAAAAAAAA",
                Customer = existingCustomer,
                Vehicle = firstVehicle,
                Amount = 10_000_000m,
                Installments = EInstallmentsTerm.Months12,
                DateCreation = DateTime.UtcNow.AddMonths(-2)
            },
            new Loan
            {
                Id = 2,
                Reference = "LN-BBBBBBBBBB",
                Customer = existingCustomer,
                Vehicle = secondVehicle,
                Amount = 8_000_000m,
                Installments = EInstallmentsTerm.Months6,
                DateCreation = DateTime.UtcNow
            }
        };

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync(existingCustomer);

        mockLoanRepository
            .Setup(r => r.GetByCustomerDocumentNumber(documentNumber))
            .ReturnsAsync(existingLoans);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act
        IEnumerable<LoanResponse> result = await service.GetByCustomer(EDocumentType.CedulaCiudadania, documentNumber);

        // Assert
        List<LoanResponse> responses = result.ToList();

        Assert.Equal(2, responses.Count);
        Assert.Contains(responses, r => r.Reference == "LN-AAAAAAAAAA" && r.VehicleIdentifier == "MK-1299");
        Assert.Contains(responses, r => r.Reference == "LN-BBBBBBBBBB" && r.VehicleIdentifier == "MK-4521");
        Assert.All(responses, r => Assert.Equal(documentNumber, r.CustomerDocumentNumber));

        mockLoanRepository.Verify(
            r => r.GetByCustomerDocumentNumber(documentNumber),
            Times.Once
        );
    }

    // [Customer has no registered loans]
    [Fact]
    public async Task GetByCustomer_NoLoans_ReturnsEmptyCollection()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const int documentNumber = 123456789;

        Customer existingCustomer = new Customer
        {
            Id = 1,
            DocumentType = EDocumentType.CedulaCiudadania,
            DocumentNumber = documentNumber,
            Name = "Michael",
            Lastname = "Olise",
            Age = 25,
            Address = "CL 105E 41T, Paris"
        };

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync(existingCustomer);

        mockLoanRepository
            .Setup(r => r.GetByCustomerDocumentNumber(documentNumber))
            .ReturnsAsync(new List<Loan>());

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act
        IEnumerable<LoanResponse> result = await service.GetByCustomer(EDocumentType.CedulaCiudadania, documentNumber);

        // Assert
        Assert.Empty(result);

        mockLoanRepository.Verify(
            r => r.GetByCustomerDocumentNumber(documentNumber),
            Times.Once
        );
    }
}