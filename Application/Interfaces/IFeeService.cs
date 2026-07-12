using CarCredit.Domain.Entities;
using CarCredit.Application.DTOs;

namespace CarCredit.Application.Interfaces;

public interface IFeeService
{
    Task<IEnumerable<Fee>> GetByCreditId(int creditId);
    Task<Fee?> RegisterPayment(int feeId, decimal amount);
    Task<CreditSummaryResponse?> GetSummary(int creditId);
    Task<IEnumerable<OverdueFeeResponse>> GetOverdue();
}