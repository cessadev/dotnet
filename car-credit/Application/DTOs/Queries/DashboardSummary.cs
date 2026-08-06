namespace CarCredit.Application.DTOs.Queries;

public record DashboardSummary(
    int TotalLoans,
    int ActiveLoans,
    int PaidLoans,
    decimal TotalPortfolioValue,
    decimal TotalCollected,
    decimal TotalOverdueAmount,
    int OverdueInstallmentsCount,
    int TotalCustomers,
    int TotalVehicles,
    decimal DelinquencyRate
);