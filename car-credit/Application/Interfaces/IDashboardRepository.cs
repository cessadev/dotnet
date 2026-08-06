using CarCredit.Application.DTOs.Queries;

namespace CarCredit.Application.Interfaces;

public interface IDashboardRepository
{
    Task<DashboardSummary> GetSummary();
}