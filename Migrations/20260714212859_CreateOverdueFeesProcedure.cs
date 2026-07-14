using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace car_credit.Migrations
{
    /// <inheritdoc />
    public partial class CreateOverdueFeesProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE PROCEDURE usp_GetOverdueFees
                AS
                BEGIN
                    SELECT f.Id, f.NumberFee, f.ValueFee, f.DateExpiration,
                        cu.Name + ' ' + cu.Lastname AS Customer, c.Vehicle,
                        DATEDIFF(DAY, f.DateExpiration, GETUTCDATE()) AS DaysOverdue
                    FROM Fees f
                    INNER JOIN Credits c ON c.Id = f.CreditId
                    INNER JOIN Customers cu ON cu.Id = c.CustomerId
                    WHERE f.Paid = 0 AND f.DateExpiration < GETUTCDATE()
                    ORDER BY DaysOverdue DESC
                END
                """
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE usp_GetOverdueFees");
        }
    }
}
