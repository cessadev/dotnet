using CarCredit.Domain.Entities;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Services;

public class FeeService : IFeeService
{
    private readonly IFeeRepository _repository;
    
    public FeeService(IFeeRepository _r)
    {
        _repository = _r;
    }

    public async Task<IEnumerable<FeeResponse?>> GetByCreditId(int creditId)
    {
        IEnumerable<Fee> fees = await _repository.GetByCreditId(creditId);
        
        return fees is null ? null : fees.Select(f => new FeeResponse(
            f.Id,
            f.CreditId,
            f.NumberFee,
            f.ValueFee,
            f.ValueFeePaid,
            f.DateExpiration,
            f.DatePayment,
            f.Paid
        ));
    }
    
    public async Task<FeeResponse?> RegisterPayment(int feeId, decimal amount)
    {
        Fee? fee = await _repository.GetById(feeId);
        if (fee is null) return null;

        fee.ValueFeePaid = amount;
        fee.DatePayment = DateTime.UtcNow;
        fee.Paid = true;

        await _repository.SaveChanges();

        return new FeeResponse(
            fee.Id,
            fee.CreditId,
            fee.NumberFee,
            fee.ValueFee,
            fee.ValueFeePaid,
            fee.DateExpiration,
            fee.DatePayment,
            fee.Paid
        );
    }

    public async Task<CreditSummary?> GetSummary(int creditId) => await _repository.GetSummary(creditId);

    public async Task<IEnumerable<OverdueFee>> GetOverdue() => await _repository.GetOverdue();
}