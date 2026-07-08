using CarCredit.Models;

namespace CarCredit.Interfaces;

public interface IFeeService
{
    Task<IEnumerable<Fee>> GetByCreditId(int creditId);
    Task<Fee?> RegisterPayment(int feeId, decimal amount);
    Task<IEnumerable<dynamic>> GetSummary(int creditId);
    Task<IEnumerable<dynamic>> GetOverdue();
}