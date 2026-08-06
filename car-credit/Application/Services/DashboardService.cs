using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.Interfaces;

namespace CarCredit.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardService(IDashboardRepository dashboardRepository)
    {
        _dashboardRepository = dashboardRepository;
    }

    public async Task<DashboardSummary> GetSummary() => await _dashboardRepository.GetSummary();
}