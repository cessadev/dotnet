using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCreditTests.Application.Services;

public class DeleteLoanTests
{
    // [Loan not found]
    [Fact]
    public async Task Delete_LoanDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string loanReference = "LN-ABC1234567";

        mockLoanRepository
            .Setup(r => r.GetByReference(loanReference))
            .ReturnsAsync((Loan?)null);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act
        bool result = await service.Delete(loanReference);

        // Assert
        Assert.False(result);

        mockLoanRepository.Verify(
            r => r.GetByReference(loanReference),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.HasUnpaidInstallments(It.IsAny<string>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.Remove(It.IsAny<Loan>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Unpaid installments]
    [Fact]
    public async Task Delete_LoanHasUnpaidInstallments_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string loanReference = "LN-ABC1234567";

        var existingLoan = new Loan
        {
            Id = 1,
            Reference = loanReference,
            Amount = 10_000_000m,
            Installments = EInstallmentsTerm.Months12
        };

        mockLoanRepository
            .Setup(r => r.GetByReference(loanReference))
            .ReturnsAsync(existingLoan);

        mockLoanRepository
            .Setup(r => r.HasUnpaidInstallments(loanReference))
            .ReturnsAsync(true);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.Delete(loanReference)
        );

        mockLoanRepository.Verify(
            r => r.GetByReference(loanReference),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.HasUnpaidInstallments(loanReference),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.Remove(It.IsAny<Loan>()),
            Times.Never
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [All installments paid]
    [Fact]
    public async Task Delete_LoanAllInstallmentsPaid_DeletesLoan()
    {
        // Arrange
        var mockLoanRepository = new Mock<ILoanRepository>();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        var mockVehicleRepository = new Mock<IVehicleRepository>();

        const string loanReference = "LN-ABC1234567";

        var existingLoan = new Loan
        {
            Id = 1,
            Reference = loanReference,
            Amount = 10_000_000m,
            Installments = EInstallmentsTerm.Months12
        };

        mockLoanRepository
            .Setup(r => r.GetByReference(loanReference))
            .ReturnsAsync(existingLoan);

        mockLoanRepository
            .Setup(r => r.HasUnpaidInstallments(loanReference))
            .ReturnsAsync(false);

        mockLoanRepository
            .Setup(r => r.Remove(existingLoan))
            .Returns(Task.CompletedTask);

        mockLoanRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        LoanService service = new LoanService(
            mockLoanRepository.Object,
            mockCustomerRepository.Object,
            mockVehicleRepository.Object
        );

        // Act
        bool result = await service.Delete(loanReference);

        // Assert
        Assert.True(result);

        mockLoanRepository.Verify(
            r => r.GetByReference(loanReference),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.HasUnpaidInstallments(loanReference),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.Remove(existingLoan),
            Times.Once
        );

        mockLoanRepository.Verify(
            r => r.SaveChanges(),
            Times.Once
        );
    }
}