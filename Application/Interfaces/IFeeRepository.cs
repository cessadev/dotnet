using CarCredit.Domain.Entities;
using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface IFeeRepository
{
    Task<Fee?> GetById(int feeId);
    Task<IEnumerable<Fee>> GetByCreditId(int creditId);
    Task SaveChanges();
    Task<CreditSummary?> GetSummary(int creditId);
    Task<IEnumerable<OverdueFee>> GetOverdue();
}