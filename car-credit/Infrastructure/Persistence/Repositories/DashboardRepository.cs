using Dapper;
using Microsoft.Data.SqlClient;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.Interfaces;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly string _connectionString;

    public DashboardRepository(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task<DashboardSummary> GetSummary()
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            WITH LoanStatus AS (
                SELECT
                    l.Id,
                    l.TotalAmount AS Amount, -- principal + interest: reflects the real value of the operation
                    CASE WHEN SUM(CASE WHEN i.Paid = 0 THEN 1 ELSE 0 END) = 0 THEN 1 ELSE 0 END AS IsFullyPaid
                FROM Loans l
                INNER JOIN Installments i ON i.LoanId = l.Id
                GROUP BY l.Id, l.TotalAmount
            ),
            Totals AS (
                SELECT
                    (SELECT COUNT(*) FROM LoanStatus)                                       AS TotalLoans,
                    (SELECT COUNT(*) FROM LoanStatus WHERE IsFullyPaid = 0)                  AS ActiveLoans,
                    (SELECT COUNT(*) FROM LoanStatus WHERE IsFullyPaid = 1)                  AS PaidLoans,
                    (SELECT ISNULL(SUM(Amount), 0) FROM LoanStatus)                          AS TotalPortfolioValue,
                    (SELECT ISNULL(SUM(AmountPaid), 0) FROM Installments WHERE Paid = 1)     AS TotalCollected,
                    (SELECT ISNULL(SUM(Amount), 0) FROM Installments
                        WHERE Paid = 0 AND DateExpiration < GETUTCDATE())                    AS TotalOverdueAmount,
                    (SELECT COUNT(*) FROM Installments
                        WHERE Paid = 0 AND DateExpiration < GETUTCDATE())                    AS OverdueInstallmentsCount,
                    (SELECT COUNT(*) FROM Customers)                                         AS TotalCustomers,
                    (SELECT COUNT(*) FROM Vehicles)                                          AS TotalVehicles
            )
            SELECT
                TotalLoans, ActiveLoans, PaidLoans, TotalPortfolioValue, TotalCollected,
                TotalOverdueAmount, OverdueInstallmentsCount, TotalCustomers, TotalVehicles,
                CASE WHEN TotalPortfolioValue = 0 THEN 0
                     ELSE CAST(TotalOverdueAmount AS DECIMAL(18,4)) / TotalPortfolioValue * 100
                END AS DelinquencyRate
            FROM Totals
            """;

        return await connection.QuerySingleAsync<DashboardSummary>(sql);
    }
}