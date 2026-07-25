using Dapper;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using CarCredit.Application.DTOs.Queries;
using CarCredit.Application.Interfaces;
using CarCredit.Infrastructure.Persistence;
using CarCredit.Domain.Entities;

namespace CarCredit.Infrastructure.Persistence.Repositories;

public class InstallmentRepository : IInstallmentRepository
{
    private readonly AppDbContext _db;
    private readonly string _connectionString;

    public InstallmentRepository(AppDbContext _database, IConfiguration config)
    {
        _db = _database;
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task<Installment?> GetByPaymentReference(string paymentReference)
        => await _db.Installments
            .Include(i => i.Loan)
            .FirstOrDefaultAsync(i => i.PaymentReference == paymentReference);

    public async Task<IEnumerable<Installment>> GetByLoanReference(string loanReference)
        => await _db.Installments
            .Include(i => i.Loan)
            .Where(i => i.Loan.Reference == loanReference)
            .OrderBy(i => i.Number)
            .ToListAsync();

    public async Task<LoanSummary?> GetSummaryByLoanReference(string loanReference)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            SELECT
                l.Reference,
                cu.Name + ' ' + cu.Lastname AS Customer,
                v.Identifier               AS Vehicle,
                COUNT(i.Id)                                          AS TotalInstallments,
                SUM(CASE WHEN i.Paid = 1 THEN 1 ELSE 0 END)          AS InstallmentsPaid,
                SUM(CASE WHEN i.Paid = 0 THEN 1 ELSE 0 END)          AS InstallmentsOwed,
                SUM(i.Amount)                                        AS TotalValue,
                SUM(CASE WHEN i.Paid = 1 THEN i.AmountPaid ELSE 0 END) AS TotalPaid,
                SUM(CASE WHEN i.Paid = 0 THEN i.Amount     ELSE 0 END) AS TotalOwed
            FROM Loans l
            INNER JOIN Customers cu ON cu.Id = l.CustomerId
            INNER JOIN Vehicles v   ON v.Id  = l.VehicleId
            INNER JOIN Installments i ON i.LoanId = l.Id
            WHERE l.Reference = @LoanReference
            GROUP BY l.Reference, cu.Name, cu.Lastname, v.Identifier
            """;

        return await connection.QuerySingleOrDefaultAsync<LoanSummary>(
            sql, new { LoanReference = loanReference });
    }

    public async Task<IEnumerable<OverdueInstallment>> GetOverdueByLoanReference(string loanReference)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            SELECT
                l.Reference AS LoanReference, i.Number, i.Amount, i.DateExpiration,
                cu.Name + ' ' + cu.Lastname AS Customer, v.Identifier AS Vehicle,
                DATEDIFF(DAY, i.DateExpiration, GETUTCDATE()) AS DaysOverdue
            FROM Installments i
            INNER JOIN Loans l      ON l.Id  = i.LoanId
            INNER JOIN Customers cu ON cu.Id = l.CustomerId
            INNER JOIN Vehicles v   ON v.Id  = l.VehicleId
            WHERE l.Reference = @LoanReference
              AND i.Paid = 0 AND i.DateExpiration < GETUTCDATE()
            ORDER BY DaysOverdue DESC
            """;

        return await connection.QueryAsync<OverdueInstallment>(
            sql, new { LoanReference = loanReference });
    }

    public async Task<IEnumerable<OverdueInstallment>> GetAllOverdue()
    {
        using var connection = new SqlConnection(_connectionString);

        return await connection.QueryAsync<OverdueInstallment>(
            "usp_GetOverdueInstallments",
            commandType: CommandType.StoredProcedure
        );
    }

    public Task AddPayment(Payment payment)
    {
        _db.Payments.Add(payment);
        return Task.CompletedTask;
    }

    public async Task SaveChanges() => await _db.SaveChangesAsync();
}