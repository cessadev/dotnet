using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace car_credit.Migrations
{
    /// <inheritdoc />
    public partial class AddFees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreditId = table.Column<int>(type: "int", nullable: false),
                    NumberFee = table.Column<int>(type: "int", nullable: false),
                    ValueFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ValueFeePaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateExpiration = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePayment = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Paid = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fees_Credits_CreditId",
                        column: x => x.CreditId,
                        principalTable: "Credits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fees_CreditId",
                table: "Fees",
                column: "CreditId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fees");
        }
    }
}
