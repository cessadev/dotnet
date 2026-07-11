using CarCredit.Application.DTOs;
using CarCredit.Application.Interfaces;
using CarCredit.Domain.Entities;

namespace CarCredit.Application.Services;

public class CreditService : ICreditService
{
    private readonly ICreditRepository _repository;

    public CreditService(ICreditRepository _r)
    {
        _repository = _r;
    }

    public async Task<Credit> Create(CreateCreditRequest request)
    {
        if (request.Fee <= 0)
            throw new ArgumentException(
                "The number of installments must be greater than zero.");

        var customer = await _repository.CustomerExists(request.CustomerId);
        if (!customer)
            throw new KeyNotFoundException($"Customer {request.CustomerId} not found.");

        var credit = new Credit
        {
            CustomerId = request.CustomerId,
            Vehicle = request.Vehicle,
            ValueCredit = request.ValueCredit,
            Fee = request.Fee
        };

        decimal installment = Math.Floor(request.ValueCredit / request.Fee * 100) / 100;
        decimal remaining = request.ValueCredit;

        var fees = new List<Fee>();

        for (int i = 1; i <= request.Fee; i++)
        {
            decimal value;

            if (i == request.Fee)
            {
                // The last installment receives the remaining funds
                value = remaining;
            }
            else
            {
                value = installment;
                remaining -= value;
            }

            fees.Add(new Fee
            {
               Credit = credit,
               NumberFee = i,
               ValueFee = value,
               DateExpiration = DateTime.UtcNow.AddMonths(i) 
            });
        }
        
        await _repository.AddCreditWithFees(credit, fees);

        return credit;
    }

    public async Task<IEnumerable<Credit>> GetAll() => await _repository.GetAll();

    public async Task<Credit?> GetById(int creditId) => await _repository.GetById(creditId);

    public async Task<bool> Delete(int creditId)
    {
        var credit = await _repository.GetByIdWithFees(creditId);
        if (credit is null) return false;

        bool allPaid = credit.Fees.All(f => f.Paid);

        if (!allPaid)
            throw new InvalidOperationException(
                "The credit cannot be deleted because it has unpaid fees.");

        await _repository.Remove(credit);
        await _repository.SaveChanges();

        return true;
    }
}