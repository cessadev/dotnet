using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class UpdateCustomerTests
{
    // [Customer does not exist]
    [Fact]
    public async Task Update_CustomerDoesNotExist_ReturnsNull()
    {
        // Arrange
        var mockCustomerRepository = new Mock<ICustomerRepository>();

        const int documentNumber = 123456789;

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync((Customer?)null);

        CustomerService service = new CustomerService(
            mockCustomerRepository.Object
        );

        UpdateCustomerRequest request = new UpdateCustomerRequest(
            Name: "Carlos",
            Lastname: "Rodríguez",
            Age: 35,
            Address: "Calle 45 #12-30"
        );

        // Act
        CustomerResponse? result = await service.Update(documentNumber, request);

        // Assert
        Assert.Null(result);

        mockCustomerRepository.Verify(
            r => r.GetByDocumentNumber(documentNumber),
            Times.Once
        );

        mockCustomerRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Valid update]
    [Fact]
    public async Task Update_ValidRequest_UpdatesFieldsAndReturnsCustomerResponse()
    {
        // Arrange
        var mockCustomerRepository = new Mock<ICustomerRepository>();

        const int documentNumber = 123456789;

        Customer existingCustomer = new Customer
        {
            Id = 1,
            DocumentType = EDocumentType.CedulaCiudadania,
            DocumentNumber = documentNumber,
            Name = "Carlos",
            Lastname = "Ramírez",
            Age = 30,
            Address = "Calle 10 #5-20"
        };

        mockCustomerRepository
            .Setup(r => r.GetByDocumentNumber(documentNumber))
            .ReturnsAsync(existingCustomer);

        mockCustomerRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        CustomerService service = new CustomerService(
            mockCustomerRepository.Object
        );

        UpdateCustomerRequest request = new UpdateCustomerRequest(
            Name: "Carlos",
            Lastname: "Rodríguez",
            Age: 35,
            Address: "Calle 45 #12-30"
        );

        // Act
        CustomerResponse? result = await service.Update(documentNumber, request);

        // Assert
        Assert.NotNull(result);

        // The DocumentType/DocumentNumber must remain unchanged
        Assert.Equal(EDocumentType.CedulaCiudadania, existingCustomer.DocumentType);
        Assert.Equal(documentNumber, existingCustomer.DocumentNumber);

        Assert.Equal(request.Name, existingCustomer.Name);
        Assert.Equal(request.Lastname, existingCustomer.Lastname);
        Assert.Equal(request.Age, existingCustomer.Age);
        Assert.Equal(request.Address, existingCustomer.Address);

        Assert.Equal(request.Name, result!.Name);
        Assert.Equal(request.Lastname, result.Lastname);
        Assert.Equal(request.Age, result.Age);
        Assert.Equal(request.Address, result.Address);

        mockCustomerRepository.Verify(
            r => r.GetByDocumentNumber(documentNumber),
            Times.Once
        );

        mockCustomerRepository.Verify(
            r => r.SaveChanges(),
            Times.Once
        );
    }
}