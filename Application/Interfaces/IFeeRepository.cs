using CarCredit.Domain.Entities;
using CarCredit.Application.DTOs;

namespace CarCredit.Application.Interfaces;

public interface IFeeRepository
{
    Task<IEnumerable<Fee>> GetByCreditId(int creditId);
    Task<Fee?> GetById(int feeId);
    Task SaveChanges();
    Task<CreditSummaryResponse?> GetSummary(int creditId);
    Task<IEnumerable<OverdueFeeResponse>> GetOverdue();
}