using Dapper;
using Microsoft.Data.SqlClient;
using CarCredit.Models;
using CarCredit.Interfaces;
using CarCredit.Data;
using Microsoft.EntityFrameworkCore;

namespace CarCredit.Services;

public class FeeService : IFeeService
{
    private readonly AppDbContext _db;
    private readonly string _connectionString;
    
    public FeeService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _connectionString = config.GetConnectionString("DefaultConnection")!;
    }

    public async Task<IEnumerable<Fee>> GetByCreditId(int creditId) 
        => await _db.Fees
            .Where(f => f.CreditId == creditId)
            .OrderBy(F => F.NumberFee)
            .ToListAsync();
    
    public async Task<Fee?> RegisterPayment(int feeId, decimal amount)
    {
        var fee = await _db.Fees.FindAsync(feeId);
        if (fee is null) return null;

        fee.ValueFeePaid = amount;
        fee.DatePayment = DateTime.UtcNow;
        fee.Paid = true;

        await _db.SaveChangesAsync();
        return fee;
    }

    public async Task<IEnumerable<dynamic>> GetSummary(int creditId)
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            SELECT
                c.Id,
                cu.Name + ' ' + cu.Lastname     AS Customer,
                c.Vehicle,
                c.Fee                                               AS TotalFees,
                SUM(CASE WHEN f.Paid = 1 THEN 1 ELSE 0 END)        AS FeesPaid,
                SUM(CASE WHEN f.Paid = 0 THEN 1 ELSE 0 END)        AS FeesOwed,
                SUM(f.ValueFee)                                     AS TotalValue,
                SUM(CASE WHEN f.Paid = 1 THEN f.ValueFeePaid ELSE 0 END) AS TotalPaid,
                SUM(CASE WHEN f.Paid = 0 THEN f.ValueFee     ELSE 0 END) AS TotalOwed
            FROM Credits c
            INNER JOIN Customers cu ON cu.Id = c.CustomerId
            INNER JOIN Fees f ON f.CreditId = c.Id
            WHERE c.Id = @CreditId
            GROUP BY c.Id, cu.Name, cu.Lastname, c.Vehicle, c.Fee
            """;

        return await connection.QueryAsync(sql, new { CreditId = creditId });
    }

    public async Task<IEnumerable<dynamic>> GetOverdue()
    {
        using var connection = new SqlConnection(_connectionString);

        var sql = """
            SELECT
                f.Id,
                f.NumberFee,
                f.ValueFee,
                f.DateExpiration,
                cu.Name + ' ' + cu.Lastname     AS Customer,
                c.Vehicle,
                DATEDIFF(DAY, f.DateExpiration, GETUTCDATE()) AS DaysOverdue
            FROM Fees f
            INNER JOIN Credits c ON c.Id = f.CreditId
            INNER JOIN Customers cu ON cu.Id = c.CustomerId
            WHERE f.Paid = 0
              AND f.DateExpiration < GETUTCDATE()
            ORDER BY DaysOverdue DESC
            """;
        
        return await connection.QueryAsync(sql);
    }
}