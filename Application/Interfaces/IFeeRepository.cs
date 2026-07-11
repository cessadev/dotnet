using CarCredit.Domain.Entities;

namespace CarCredit.Application.Interfaces;

public interface IFeeRepository
{
    Task<IEnumerable<Fee>> GetByCreditId(int creditId);
    Task<Fee?> GetById(int feeId);
    Task SaveChanges();
    Task<IEnumerable<CreditSummaryResponse>> GetSummary(int creditId);
    Task<IEnumerable<OverdueFeeResponse>> GetOverdue();
}