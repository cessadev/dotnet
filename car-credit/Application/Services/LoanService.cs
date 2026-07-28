using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

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
                $"El vehículo con identificador/placa {request.VehicleIdentifier} no fue encontrado.");

        if (request.Amount > vehicle.MarketValue)
            throw new InvalidOperationException(
                $"El monto del préstamo no puede exceder el valor de mercado del vehículo ({vehicle.MarketValue:C}).");
        
        string reference = $"LN-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";

        Loan loan = new Loan
        {
            Reference = reference,
            CustomerId = customer.Id,
            Customer = customer,
            VehicleId = vehicle.Id,
            Vehicle = vehicle,
            Amount = request.Amount,
            Installments = request.Installments
        };

        int totalInstallments = (int)request.Installments;
        decimal installmentValue = Math.Floor(request.Amount / totalInstallments * 100) / 100;
        decimal remaining = request.Amount;

        List<Installment> installments = new List<Installment>();

        for (int i = 1; i <= totalInstallments; i++)
        {
            decimal value = i == totalInstallments ? remaining : installmentValue;
            if (i != totalInstallments) remaining -= value;

            installments.Add(new Installment
            {
                Loan = loan,
                Number = i,
                PaymentReference = $"{reference}-{i:D2}",
                Amount = value,
                DateExpiration = DateTime.UtcNow.AddMonths(i)
            });
        }

        await _loanRepository.Add(loan);
        await _loanRepository.AddInstallments(installments);
        await _loanRepository.SaveChanges();

        return ToResponse(loan);
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

    private static LoanResponse ToResponse(Loan l) => new(
        l.Reference,
        l.Customer.DocumentNumber,
        l.Vehicle.Identifier,
        l.Amount,
        l.Installments,
        l.DateCreation
    );
}