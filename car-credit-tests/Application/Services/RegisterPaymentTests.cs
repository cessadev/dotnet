using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

namespace CarCreditTests.Application.Services;

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

    // [Partial payment / abono]
    [Fact]
    public async Task RegisterPayment_PartialAmount_RegistersAbonoAndKeepsInstallmentUnpaid()
    {
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();
        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan { Id = 1, Reference = "LN-ABC1234567" };

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
        mockInstallmentRepository.Setup(r => r.AddPayment(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        mockInstallmentRepository.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

        InstallmentService service = new InstallmentService(mockInstallmentRepository.Object);

        // Act
        InstallmentResponse? result = await service.RegisterPayment(paymentReference, EPaymentMethod.Cash, 500_000m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(500_000m, existingInstallment.AmountPaid);
        Assert.False(existingInstallment.Paid);
        Assert.NotNull(existingInstallment.DatePayment);

        mockInstallmentRepository.Verify(r => r.AddPayment(It.IsAny<Payment>()), Times.Once);
        mockInstallmentRepository.Verify(r => r.SaveChanges(), Times.Once);
    }

    // [Second abono completes the installment]
    [Fact]
    public async Task RegisterPayment_SecondAbonoCoversRemainingBalance_MarksInstallmentAsPaid()
    {
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();
        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan { Id = 1, Reference = "LN-ABC1234567" };

        Installment existingInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = paymentReference,
            Amount = 833_333.33m,
            AmountPaid = 500_000m,
            DateExpiration = DateTime.UtcNow.AddMonths(1),
            DatePayment = DateTime.UtcNow.AddDays(-1),
            Paid = false,
            Loan = existingLoan
        };

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync(existingInstallment);
        mockInstallmentRepository.Setup(r => r.AddPayment(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        mockInstallmentRepository.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

        InstallmentService service = new InstallmentService(mockInstallmentRepository.Object);

        // Act
        InstallmentResponse? result = await service.RegisterPayment(paymentReference, EPaymentMethod.Cash, 333_333.33m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(833_333.33m, existingInstallment.AmountPaid);
        Assert.True(existingInstallment.Paid);
    }

    // [Overpayment blocked]
    [Fact]
    public async Task RegisterPayment_AmountExceedsRemainingBalance_ThrowsInvalidOperationException()
    {
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();
        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan { Id = 1, Reference = "LN-ABC1234567" };

        Installment existingInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = paymentReference,
            Amount = 833_333.33m,
            AmountPaid = 500_000m,
            DateExpiration = DateTime.UtcNow.AddMonths(1),
            DatePayment = DateTime.UtcNow.AddDays(-1),
            Paid = false,
            Loan = existingLoan
        };

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync(existingInstallment);

        InstallmentService service = new InstallmentService(mockInstallmentRepository.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.RegisterPayment(paymentReference, EPaymentMethod.Cash, 400_000m)
        );

        Assert.Equal(500_000m, existingInstallment.AmountPaid);
        Assert.False(existingInstallment.Paid);

        mockInstallmentRepository.Verify(r => r.AddPayment(It.IsAny<Payment>()), Times.Never);
        mockInstallmentRepository.Verify(r => r.SaveChanges(), Times.Never);
    }

    [Fact]
    public async Task RegisterPayment_AmountWithFloatingPointNoiseEqualsRemainingBalance_MarksInstallmentAsPaid()
    {
        var mockInstallmentRepository = new Mock<IInstallmentRepository>();
        const string paymentReference = "LN-ABC1234567-01";

        Loan existingLoan = new Loan { Id = 1, Reference = "LN-ABC1234567" };

        Installment existingInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = paymentReference,
            Amount = 833_333.33m,
            AmountPaid = 755_555.56m,
            DateExpiration = DateTime.UtcNow.AddMonths(1),
            DatePayment = DateTime.UtcNow.AddDays(-1),
            Paid = false,
            Loan = existingLoan
        };

        mockInstallmentRepository
            .Setup(r => r.GetByPaymentReference(paymentReference))
            .ReturnsAsync(existingInstallment);
        mockInstallmentRepository.Setup(r => r.AddPayment(It.IsAny<Payment>())).Returns(Task.CompletedTask);
        mockInstallmentRepository.Setup(r => r.SaveChanges()).Returns(Task.CompletedTask);

        InstallmentService service = new InstallmentService(mockInstallmentRepository.Object);

        // Act
        InstallmentResponse? result = await service.RegisterPayment(paymentReference, EPaymentMethod.Cash, 77_777.7700000001m);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(833_333.33m, existingInstallment.AmountPaid);
        Assert.True(existingInstallment.Paid);
    }
}