using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

public class RegisterPaymentTests
{
    // [Installment not found]
    [Fact]
    public async Task RegisterPayment_InstallmentDoesNotExist_ReturnsNull()
    {
        // Arrange
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();

        const string paymentReference = "LN-ABC1234567-01";

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync((Installment?)null);

        InstallmentService service = new InstallmentService(
            mockInstallmentRepository.Object
        );

        // Act
        InstallmentResponse? result = await service.RegisterPayment(
            paymentReference,
            EPaymentMethod.Cash,
            833_333.33m
        );

        // Assert
        Assert.Null(result);

        mockInstallmentRepository.Verify(
            r => r.GetByPaymentReference(paymentReference),
            Times.Once
        );

        mockInstallmentRepository.Verify(
            r => r.AddPayment(It.IsAny<Payment>()),
            Times.Never
        );

        mockInstallmentRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }

    // [Valid payment]
    [Fact]
    public async Task RegisterPayment_ValidPayment_RegistersPaymentAndMarksInstallmentAsPaid()
    {
        // Arrange
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();

        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan
        {
            Id = 1,
            Reference = "LN-ABC1234567"
        };

        Installment existingInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = paymentReference,
            Amount = 833_333.33m,
            AmountPaid = 0m,
            DateExpiration = DateTime.UtcNow.AddMonths(1),
            DatePayment = null,
            Paid = false,
            Loan = existingLoan
        };

        Payment? capturedPayment = null;

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync(existingInstallment);

        mockInstallmentRepository
            .Setup(r => r.AddPayment(It.IsAny<Payment>()))
            .Callback<Payment>(
                payment => capturedPayment = payment)
            .Returns(Task.CompletedTask);

        mockInstallmentRepository
            .Setup(r => r.SaveChanges())
            .Returns(Task.CompletedTask);

        InstallmentService service = new InstallmentService(
            mockInstallmentRepository.Object
        );

        // Act
        InstallmentResponse? result = await service.RegisterPayment(
            paymentReference,
            EPaymentMethod.Cash,
            833_333.33m
        );

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedPayment);

        Assert.Equal(833_333.33m, existingInstallment.AmountPaid);
        Assert.True(existingInstallment.Paid);
        Assert.NotNull(existingInstallment.DatePayment);

        Assert.Equal(833_333.33m, capturedPayment!.Amount);
        Assert.Equal(EPaymentMethod.Cash, capturedPayment.Method);
        Assert.Equal(paymentReference, capturedPayment.ReferencePayment);
        Assert.Equal(existingInstallment.Id, capturedPayment.InstallmentId);

        Assert.StartsWith("PAY-", capturedPayment.Number);
        Assert.Equal(12, capturedPayment.Number.Length);

        Assert.NotEqual(default, capturedPayment.Date);

        mockInstallmentRepository.Verify(
            r => r.GetByPaymentReference(paymentReference),
            Times.Once
        );

        mockInstallmentRepository.Verify(
            r => r.AddPayment(It.IsAny<Payment>()),
            Times.Once
        );

        mockInstallmentRepository.Verify(
            r => r.SaveChanges(),
            Times.Once
        );
    }

    // [The amount does not match the installment]
    [Fact]
    public async Task RegisterPayment_PartialPayment_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();

        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan
        {
            Id = 1,
            Reference = "LN-ABC1234567"
        };

        Installment existingInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = paymentReference,
            Amount = 833_333.33m,
            AmountPaid = 0m,
            DateExpiration = DateTime.UtcNow.AddMonths(1),
            DatePayment = null,
            Paid = false,
            Loan = existingLoan
        };

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync(existingInstallment);

        InstallmentService service = new InstallmentService(
            mockInstallmentRepository.Object
        );

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterPayment(
                paymentReference,
                EPaymentMethod.Cash,
                500_000m
            )
        );

        // Assert
        Assert.Equal(0m, existingInstallment.AmountPaid);
        Assert.False(existingInstallment.Paid);
        Assert.Null(existingInstallment.DatePayment);

        mockInstallmentRepository.Verify(
            r => r.GetByPaymentReference(paymentReference),
            Times.Once
        );

        mockInstallmentRepository.Verify(
            r => r.AddPayment(It.IsAny<Payment>()),
            Times.Never
        );

        mockInstallmentRepository.Verify(
            r => r.SaveChanges(),
            Times.Never
        );
    }
}