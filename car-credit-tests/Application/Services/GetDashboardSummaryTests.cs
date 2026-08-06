using Moq;
using Xunit;
using CarCredit.Application.Interfaces;
using CarCredit.Application.Services;
using CarCredit.Application.DTOs.Queries;

namespace CarCreditTests.Application.Services;

public class GetDashboardSummaryTests
{
    // [Portfolio with active and paid loans]
    [Fact]
    public async Task GetSummary_ReturnsSummaryFromRepository()
    {
        // Arrange
        var mockDashboardRepository = new Mock<IDashboardRepository>();

        DashboardSummary expectedSummary = new DashboardSummary(
            TotalLoans: 10,
            ActiveLoans: 7,
            PaidLoans: 3,
            TotalPortfolioValue: 350_000_000m,
            TotalCollected: 120_000_000m,
            TotalOverdueAmount: 8_500_000m,
            OverdueInstallmentsCount: 4,
            TotalCustomers: 9,
            TotalVehicles: 10,
            DelinquencyRate: 2.4286m
        );

        mockDashboardRepository
            .Setup(r => r.GetSummary())
            .ReturnsAsync(expectedSummary);

        DashboardService service = new DashboardService(
            mockDashboardRepository.Object
        );

        // Act
        DashboardSummary result = await service.GetSummary();

        // Assert
        Assert.Equal(expectedSummary, result);

        mockDashboardRepository.Verify(
            r => r.GetSummary(),
            Times.Once
        );
    }

    // [Empty portfolio]
    [Fact]
    public async Task GetSummary_EmptyPortfolio_ReturnsZeroedSummaryUnchanged()
    {
        // Arrange
        var mockDashboardRepository = new Mock<IDashboardRepository>();

        DashboardSummary emptySummary = new DashboardSummary(
            TotalLoans: 0,
            ActiveLoans: 0,
            PaidLoans: 0,
            TotalPortfolioValue: 0m,
            TotalCollected: 0m,
            TotalOverdueAmount: 0m,
            OverdueInstallmentsCount: 0,
            TotalCustomers: 0,
            TotalVehicles: 0,
            DelinquencyRate: 0m
        );

        mockDashboardRepository
            .Setup(r => r.GetSummary())
            .ReturnsAsync(emptySummary);

        DashboardService service = new DashboardService(
            mockDashboardRepository.Object
        );

        // Act
        DashboardSummary result = await service.GetSummary();

        // Assert
        Assert.Equal(0, result.TotalLoans);
        Assert.Equal(0m, result.TotalPortfolioValue);
        Assert.Equal(0m, result.DelinquencyRate);

        mockDashboardRepository.Verify(
            r => r.GetSummary(),
            Times.Once
        );
    }
}