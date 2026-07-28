using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace car_credit.Migrations
{
    /// <inheritdoc />
    public partial class FixOverdueInstallmentsProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER PROCEDURE usp_GetOverdueInstallments
                AS
                BEGIN
                    SELECT
                        l.Reference AS LoanReference, i.Number, i.Amount, i.DateExpiration,
                        cu.Name + ' ' + cu.Lastname AS Customer, v.Identifier AS Vehicle,
                        DATEDIFF(DAY, i.DateExpiration, GETUTCDATE()) AS DaysOverdue
                    FROM Installments i
                    INNER JOIN Loans l      ON l.Id  = i.LoanId
                    INNER JOIN Customers cu ON cu.Id = l.CustomerId
                    INNER JOIN Vehicles v   ON v.Id  = l.VehicleId
                    WHERE i.Paid = 0 AND i.DateExpiration < GETUTCDATE()
                    ORDER BY DaysOverdue DESC
                END
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE usp_GetOverdueInstallments");
        }
    }
}
