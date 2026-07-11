using CarCredit.Domain.Entities;
using CarCredit.Application.Interfaces;
using CarCredit.Application.DTOs;

namespace CarCredit.Application.Services;

public class FeeService : IFeeService
{
    private readonly IFeeRepository _repository;
    
    public FeeService(IFeeRepository _r)
    {
        _repository = _r;
    }

    public async Task<IEnumerable<Fee>> GetByCreditId(int creditId) => await _repository.GetByCreditId(creditId);
    
    public async Task<Fee?> RegisterPayment(int feeId, decimal amount)
    {
        var fee = await _repository.GetById(feeId);
        if (fee is null) return null;

        fee.ValueFeePaid = amount;
        fee.DatePayment = DateTime.UtcNow;
        fee.Paid = true;

        await _repository.SaveChanges();

        return fee;
    }

    public async Task<IEnumerable<CreditSummaryResponse>> GetSummary(int creditId) => await _repository.GetSummary(creditId);

    public async Task<IEnumerable<OverdueFeeResponse>> GetOverdue() => await _repository.GetOverdue();
}