using Moq;
using Xunit;
using CarCredit.Application.Services;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

namespace CarCreditTests.Application.Services;

public class CreateLoanTests
{
    // [Customer does not exist]
    [Fact]
    public async Task Create_CustomerDoesNotExist_ThrowsKeyNotFoundException()
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

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(123456789))
            .ReturnsAsync((Customer?)null);

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier(existingVehicle.Identifier))
            .ReturnsAsync(existingVehicle);

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        CreateLoanRequest request = new CreateLoanRequest(
            CustomerDocumentNumber: 123456789,
            VehicleIdentifier: existingVehicle.Identifier,
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.Create(request)
        );

        mockLoanRepository.Verify(
            r => r.Add(It.IsAny<Loan>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Vehicle does not exist]
    [Fact]
    public async Task Create_VehicleDoesNotExist_ThrowsKeyNotFoundException()
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

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(existingCustomer.DocumentNumber))
            .ReturnsAsync(existingCustomer);

        mockVehicleRepository
            .Setup(r => r.GetByIdentifier("MK-1299"))
            .ReturnsAsync((Vehicle?)null);

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        CreateLoanRequest request = new CreateLoanRequest(
            CustomerDocumentNumber: existingCustomer.DocumentNumber,
            VehicleIdentifier: "MK-1299",
            Amount: 10_000_000m,
            Installments: EInstallmentsTerm.Months12
        );

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => service.Create(request)
        );

        mockLoanRepository.Verify(
            r => r.Add(It.IsAny<Loan>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Valid request + rounding, with interest applied]
    [Fact]
    public async Task Create_ValidRequest_DistributesRoundingRemainderCorrectly()
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

        // 10_000_000 at 12 months (2.8%) => interest 280_000 => total 10_280_000
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
        Assert.Equal(10_280_000m, list.Sum(i => i.Amount));

        Assert.All(
            list.Take(11),
            installment => Assert.Equal(856_666.66m, installment.Amount)
        );

        Assert.Equal(856_666.74m, list[11].Amount);
    }

    // [Interest rate and total amount stored on the loan]
    [Fact]
    public async Task Create_ValidRequest_StoresInterestRateAndTotalAmountOnLoan()
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

        Vehicle existingVehicle = new Vehicle
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

        mockLoanRepository.Setup(r => r.Add(It.IsAny<Loan>())).Returns(Task.CompletedTask);
        mockLoanRepository.Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>())).Returns(Task.CompletedTask);
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
        Assert.Equal(10_000_000m, result.Amount);
        Assert.Equal(0.028m, result.InterestRate);
        Assert.Equal(10_280_000m, result.TotalAmount);
    }

    // [Every term maps to its configured interest rate]
    [Theory]
    [InlineData(EInstallmentsTerm.Months6, 0.015)]
    [InlineData(EInstallmentsTerm.Months12, 0.028)]
    [InlineData(EInstallmentsTerm.Months18, 0.042)]
    [InlineData(EInstallmentsTerm.Months24, 0.068)]
    [InlineData(EInstallmentsTerm.Months30, 0.082)]
    [InlineData(EInstallmentsTerm.Months36, 0.115)]
    [InlineData(EInstallmentsTerm.Months42, 0.154)]
    [InlineData(EInstallmentsTerm.Months48, 0.183)]
    public async Task Create_EachInstallmentTerm_AppliesConfiguredInterestRate(
        EInstallmentsTerm term, double expectedRate)
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

        Vehicle existingVehicle = new Vehicle
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

        mockLoanRepository.Setup(r => r.Add(It.IsAny<Loan>())).Returns(Task.CompletedTask);
        mockLoanRepository.Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>())).Returns(Task.CompletedTask);
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
            Installments: term
        );

        // Act
        LoanResponse result = await service.Create(request);

        // Assert
        Assert.Equal((decimal)expectedRate, result.InterestRate);
    }

    // [Loan amount exceeds the value of the vehicle]
    [Fact]
    public async Task Create_AmountExceedsVehicleMarketValue_ThrowsInvalidOperationException()
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

        Vehicle existingVehicle = new Vehicle
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

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        CreateLoanRequest request = new CreateLoanRequest(
            CustomerDocumentNumber: existingCustomer.DocumentNumber,
            VehicleIdentifier: existingVehicle.Identifier,
            Amount: 60_000_000m,
            Installments: EInstallmentsTerm.Months12
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.Create(request)
        );

        mockLoanRepository.Verify(
            r => r.Add(It.IsAny<Loan>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.AddInstallments(It.IsAny<IEnumerable<Installment>>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
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
    public async Task Create_ValidInstallmentTerm_CreatesExpectedNumberOfInstallments(
        EInstallmentsTerm installmentTerm,
        int expectedInstallments)
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

        Vehicle existingVehicle = new Vehicle
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
            .Callback<IEnumerable<Installment>>(
                installments => capturedInstallments = installments)
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        CreateLoanRequest request = new CreateLoanRequest(
            CustomerDocumentNumber: existingCustomer.DocumentNumber,
            VehicleIdentifier: existingVehicle.Identifier,
            Amount: 10_000_000m,
            Installments: installmentTerm
        );

        // Act
        LoanResponse result = await service.Create(request);

        // Assert
        Assert.NotNull(capturedInstallments);

        var list = capturedInstallments!.ToList();

        Assert.Equal(expectedInstallments, list.Count);
    }

    // [Verify the generation of loan references and installment payments]
    [Fact]
    public async Task Create_ValidRequest_GeneratesCorrectLoanAndInstallmentReferences()
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

        Vehicle existingVehicle = new Vehicle
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
            .Callback<IEnumerable<Installment>>(
                installments => capturedInstallments = installments)
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

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

        Assert.StartsWith("LN-", result.Reference);
        Assert.Equal(13, result.Reference.Length);

        Assert.Equal(
            $"{result.Reference}-01",
            list[0].PaymentReference
        );

        Assert.Equal(
            $"{result.Reference}-02",
            list[1].PaymentReference
        );

        Assert.Equal(
            $"{result.Reference}-12",
            list[11].PaymentReference
        );

        Assert.All(
            list,
            installment => Assert.StartsWith(
                $"{result.Reference}-",
                installment.PaymentReference
            )
        );
    }

    // [Check the expiration dates]
    [Fact]
    public async Task Create_ValidRequest_GeneratesInstallmentsWithSequentialExpirationDates()
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

        Vehicle existingVehicle = new Vehicle
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
            .Callback<IEnumerable<Installment>>(
                installments => capturedInstallments = installments)
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.Add(It.IsAny<Loan>()))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

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

        for (int i = 1; i < list.Count; i++)
        {
            Assert.True(
                list[i].DateExpiration > list[i - 1].DateExpiration
            );
        }

        Assert.True(
            list[0].DateExpiration < list[11].DateExpiration
        );
    }
}