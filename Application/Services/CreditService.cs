using CarCredit.Application.DTOs.Requests;
using CarCredit.Application.DTOs.Responses;
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

    public async Task<CreditResponse> Create(CreateCreditRequest request)
    {
        if (request.Fee <= 0)
            throw new ArgumentException(
                "The number of installments must be greater than zero.");

        var customer = await _repository.CustomerExists(request.CustomerId);
        if (!customer)
            throw new KeyNotFoundException($"Customer with ID {request.CustomerId} not found.");

        Credit credit = new Credit
        {
            CustomerId = request.CustomerId,
            Vehicle = request.Vehicle,
            ValueCredit = request.ValueCredit,
            Fee = request.Fee
        };

        decimal installment = Math.Floor(request.ValueCredit / request.Fee * 100) / 100;
        decimal remaining = request.ValueCredit;

        List<Fee> fees = new List<Fee>();

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
        
        await _repository.Add(credit);
        await _repository.AddFees(fees);
        await _repository.SaveChanges();

        return new CreditResponse(
            credit.Id,
            credit.CustomerId,
            credit.Vehicle,
            credit.ValueCredit,
            credit.Fee
        );
    }

    public async Task<IEnumerable<CreditResponse>> GetAll()
    {
        IEnumerable<Credit> credits = await _repository.GetAll();

        return credits.Select(c => new CreditResponse(
            c.Id,
            c.CustomerId,
            c.Vehicle,
            c.ValueCredit,
            c.Fee
        ));
    }

    public async Task<CreditResponse?> GetById(int creditId)
    {
        Credit? credit = await _repository.GetById(creditId);
        return credit is null ? null : new CreditResponse(
            credit.Id,
            credit.CustomerId,
            credit.Vehicle,
            credit.ValueCredit,
            credit.Fee
        );
    }

    public async Task<bool> Delete(int creditId)
    {
        Credit? credit = await _repository.GetById(creditId);
        if (credit is null) return false;

        if (await _repository.HasUnpaidFees(creditId))
        {
            throw new InvalidOperationException(
                "The credit cannot be deleted because it has unpaid fees.");
        }

        await _repository.Remove(credit);
        await _repository.SaveChanges();

        return true;
    }
}