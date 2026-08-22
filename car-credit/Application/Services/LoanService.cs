using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Constants;
using CarCredit.Domain.Entities;
using CarCredit.Domain.Enums;

namespace CarCredit.Application.Services;

public class LoanService : ILoanService
{
    private readonly ILoanRepository _loanRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public LoanService(
        ILoanRepository loanRepository,
        ICustomerRepository customerRepository,
        IVehicleRepository vehicleRepository)
    {
        _loanRepository = loanRepository;
        _customerRepository = customerRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<LoanResponse> Create(CreateLoanRequest request)
    {
        Customer? customer = await _customerRepository.GetByDocumentNumber(request.CustomerDocumentNumber)
            ?? throw new KeyNotFoundException(
                $"El cliente con número de documento {request.CustomerDocumentNumber} no fue encontrado.");

        Vehicle? vehicle = await _vehicleRepository.GetByIdentifier(request.VehicleIdentifier)
            ?? throw new KeyNotFoundException(
                $"El vehículo con identificador {request.VehicleIdentifier} no fue encontrado.");

        if (request.Amount > vehicle.MarketValue)
            throw new InvalidOperationException(
                $"El monto del préstamo no puede exceder el valor de mercado del vehículo ({vehicle.MarketValue:C}).");

        string reference = $"LN-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";

        decimal interestRate = InterestRates.For(request.Installments);
        decimal interestAmount = Math.Round(request.Amount * interestRate, 2, MidpointRounding.AwayFromZero);
        decimal totalAmount = request.Amount + interestAmount;

        Loan loan = new Loan
        {
            Reference = reference,
            CustomerId = customer.Id,
            Customer = customer,
            VehicleId = vehicle.Id,
            Vehicle = vehicle,
            Amount = request.Amount,
            InterestRate = interestRate,
            TotalAmount = totalAmount,
            Installments = request.Installments
        };

        List<(int Number, decimal Amount, DateTime DateExpiration)> schedule =
            GenerateSchedule(totalAmount, request.Installments);

        List<Installment> installments = schedule.Select(s => new Installment
        {
            Loan = loan,
            Number = s.Number,
            PaymentReference = $"{reference}-{s.Number:D2}",
            Amount = s.Amount,
            DateExpiration = s.DateExpiration
        }).ToList();

        await _loanRepository.Add(loan);
        await _loanRepository.AddInstallments(installments);
        await _loanRepository.SaveChanges();

        return ToResponse(loan);
    }

    public async Task<LoanSimulation> Simulate(SimulateLoanRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.VehicleIdentifier))
        {
            Vehicle? vehicle = await _vehicleRepository.GetByIdentifier(request.VehicleIdentifier)
                ?? throw new KeyNotFoundException(
                    $"El vehículo con identificador {request.VehicleIdentifier} no fue encontrado.");

            if (request.Amount > vehicle.MarketValue)
                throw new InvalidOperationException(
                    $"El monto simulado no puede exceder el valor de mercado del vehículo ({vehicle.MarketValue:C}).");
        }

        decimal interestRate = InterestRates.For(request.Installments);
        decimal interestAmount = Math.Round(request.Amount * interestRate, 2, MidpointRounding.AwayFromZero);
        decimal totalAmount = request.Amount + interestAmount;

        List<(int Number, decimal Amount, DateTime DateExpiration)> schedule =
            GenerateSchedule(totalAmount, request.Installments);

        return new LoanSimulation(
            request.Amount,
            request.Installments,
            interestRate,
            interestAmount,
            totalAmount,
            schedule[0].Amount,
            schedule.Sum(s => s.Amount),
            schedule.Select(s => new SimulatedInstallment(s.Number, s.Amount, s.DateExpiration)).ToList()
        );
    }

    public async Task<IEnumerable<LoanResponse>> GetAll()
    {
        IEnumerable<Loan> loans = await _loanRepository.GetAll();
        return loans.Select(ToResponse);
    }

    public async Task<LoanResponse?> GetByReference(string reference)
    {
        Loan? loan = await _loanRepository.GetByReference(reference);
        return loan is null ? null : ToResponse(loan);
    }

    public async Task<IEnumerable<LoanResponse>> GetByCustomer(EDocumentType documentType, int documentNumber)
    {
        Customer? customer = await _customerRepository.GetByDocumentNumber(documentNumber)
            ?? throw new KeyNotFoundException(
                $"El cliente con número de documento {documentNumber} no fue encontrado.");

        if (customer.DocumentType != documentType)
            throw new KeyNotFoundException(
                $"El cliente con número de documento {documentNumber} no corresponde al tipo de documento indicado.");

        IEnumerable<Loan> loans = await _loanRepository.GetByCustomerDocumentNumber(documentNumber);
        return loans.Select(ToResponse);
    }

    public async Task<bool> Delete(string reference)
    {
        Loan? loan = await _loanRepository.GetByReference(reference);
        if (loan is null) return false;

        if (await _loanRepository.HasUnpaidInstallments(reference))
            throw new InvalidOperationException(
                "El préstamo registrado no puede ser eliminado porque existen cuotas pendientes por pagar.");

        await _loanRepository.Remove(loan);
        await _loanRepository.SaveChanges();

        return true;
    }

    private static List<(int Number, decimal Amount, DateTime DateExpiration)> GenerateSchedule(
        decimal amount, EInstallmentsTerm term)
    {
        int totalInstallments = (int)term;
        decimal installmentValue = Math.Floor(amount / totalInstallments * 100) / 100;
        decimal remaining = amount;

        List<(int, decimal, DateTime)> schedule = new();

        for (int i = 1; i <= totalInstallments; i++)
        {
            decimal value = i == totalInstallments ? remaining : installmentValue;
            if (i != totalInstallments) remaining -= value;

            schedule.Add((i, value, DateTime.UtcNow.AddMonths(i)));
        }

        return schedule;
    }

    private static LoanResponse ToResponse(Loan l) => new(
        l.Reference,
        l.Customer.DocumentNumber,
        l.Vehicle.Identifier,
        l.Amount,
        l.InterestRate,
        l.TotalAmount,
        l.Installments,
        l.DateCreation
    );
}