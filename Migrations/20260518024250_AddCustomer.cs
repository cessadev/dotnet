using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace car_credit.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Customer",
                table: "Credits");

            migrationBuilder.AddColumn<int>(
                name: "CustomerId",
                table: "Credits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Credits_CustomerId",
                table: "Credits",
                column: "CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Credits_Customers_CustomerId",
                table: "Credits");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropIndex(
                name: "IX_Credits_CustomerId",
                table: "Credits");

            migrationBuilder.DropColumn(
                name: "CustomerId",
                table: "Credits");

            migrationBuilder.AddColumn<string>(
                name: "Customer",
                table: "Credits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
