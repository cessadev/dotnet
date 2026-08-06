using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;
using CarCredit.Application.DTOs.Responses;

namespace CarCreditTests.Application.Services;

public class PaymentServiceTests
{
    // [Installment has registered payments]
    [Fact]
    public async Task GetByInstallmentReference_PaymentsExist_ReturnsMappedPaymentResponses()
    {
        // Arrange
        var mockPaymentRepository = new Mock<IPaymentRepository>();

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
            Loan = existingLoan
        };

        Payment existingPayment = new Payment
        {
            Id = 1,
            Number = "PAY-12345678",
            Amount = 833_333.33m,
            Method = EPaymentMethod.Cash,
            ReferencePayment = paymentReference,
            Date = DateTime.UtcNow,
            Installment = existingInstallment
        };

        mockPaymentRepository
            .Setup(r => r.GetByInstallmentReference(paymentReference))
            .ReturnsAsync(new List<Payment> { existingPayment });

        PaymentService service = new PaymentService(
            mockPaymentRepository.Object
        );

        // Act
        IEnumerable<PaymentResponse> result = await service.GetByInstallmentReference(paymentReference);

        // Assert
        PaymentResponse response = Assert.Single(result);

        Assert.Equal(existingPayment.Number, response.Number);
        Assert.Equal(existingPayment.Amount, response.Amount);
        Assert.Equal(existingPayment.Method, response.Method);
        Assert.Equal(paymentReference, response.ReferencePayment);
        Assert.Equal(existingInstallment.Number, response.InstallmentNumber);
        Assert.Equal(existingLoan.Reference, response.LoanReference);

        mockPaymentRepository.Verify(
            r => r.GetByInstallmentReference(paymentReference),
            Times.Once
        );
    }

    // [Installment has no registered payments]
    [Fact]
    public async Task GetByInstallmentReference_NoPayments_ReturnsEmptyCollection()
    {
        // Arrange
        var mockPaymentRepository = new Mock<IPaymentRepository>();

        const string paymentReference = "LN-ABC1234567-01";

        mockPaymentRepository
            .Setup(r => r.GetByInstallmentReference(paymentReference))
            .ReturnsAsync(new List<Payment>());

        PaymentService service = new PaymentService(
            mockPaymentRepository.Object
        );

        // Act
        IEnumerable<PaymentResponse> result = await service.GetByInstallmentReference(paymentReference);

        // Assert
        Assert.Empty(result);

        mockPaymentRepository.Verify(
            r => r.GetByInstallmentReference(paymentReference),
            Times.Once
        );
    }

    // [Loan has registered payments across installments]
    [Fact]
    public async Task GetByLoanReference_PaymentsExist_ReturnsMappedPaymentResponses()
    {
        // Arrange
        var mockPaymentRepository = new Mock<IPaymentRepository>();

        const string loanReference = "LN-ABC1234567";

        Loan existingLoan = new Loan
        {
            Id = 1,
            Reference = loanReference
        };

        Installment firstInstallment = new Installment
        {
            Id = 1,
            Number = 1,
            PaymentReference = $"{loanReference}-01",
            Loan = existingLoan
        };

        Installment secondInstallment = new Installment
        {
            Id = 2,
            Number = 2,
            PaymentReference = $"{loanReference}-02",
            Loan = existingLoan
        };

        List<Payment> existingPayments = new()
        {
            new Payment
            {
                Id = 1,
                Number = "PAY-11111111",
                Amount = 833_333.33m,
                Method = EPaymentMethod.Cash,
                ReferencePayment = firstInstallment.PaymentReference,
                Date = DateTime.UtcNow.AddMonths(-1),
                Installment = firstInstallment
            },
            new Payment
            {
                Id = 2,
                Number = "PAY-22222222",
                Amount = 833_333.33m,
                Method = EPaymentMethod.BankTransfer,
                ReferencePayment = secondInstallment.PaymentReference,
                Date = DateTime.UtcNow,
                Installment = secondInstallment
            }
        };

        mockPaymentRepository
            .Setup(r => r.GetByLoanReference(loanReference))
            .ReturnsAsync(existingPayments);

        PaymentService service = new PaymentService(
            mockPaymentRepository.Object
        );

        // Act
        IEnumerable<PaymentResponse> result = await service.GetByLoanReference(loanReference);

        // Assert
        List<PaymentResponse> responses = result.ToList();

        Assert.Equal(2, responses.Count);
        Assert.All(responses, r => Assert.Equal(loanReference, r.LoanReference));
        Assert.Contains(responses, r => r.InstallmentNumber == 1);
        Assert.Contains(responses, r => r.InstallmentNumber == 2);

        mockPaymentRepository.Verify(
            r => r.GetByLoanReference(loanReference),
            Times.Once
        );
    }

    // [Loan has no registered payments]
    [Fact]
    public async Task GetByLoanReference_NoPayments_ReturnsEmptyCollection()
    {
        // Arrange
        var mockPaymentRepository = new Mock<IPaymentRepository>();

        const string loanReference = "LN-ABC1234567";

        mockPaymentRepository
            .Setup(r => r.GetByLoanReference(loanReference))
            .ReturnsAsync(new List<Payment>());

        PaymentService service = new PaymentService(
            mockPaymentRepository.Object
        );

        // Act
        IEnumerable<PaymentResponse> result = await service.GetByLoanReference(loanReference);

        // Assert
        Assert.Empty(result);

        mockPaymentRepository.Verify(
            r => r.GetByLoanReference(loanReference),
            Times.Once
        );
    }
}