using CarCredit.Application.DTOs.Responses;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface IFeeService
{
    Task<IEnumerable<FeeResponse?>> GetByCreditId(int creditId);
    Task<FeeResponse?> RegisterPayment(int feeId, decimal amount);
    Task<CreditSummary?> GetSummary(int creditId);
    Task<IEnumerable<OverdueFee>> GetOverdue();
}