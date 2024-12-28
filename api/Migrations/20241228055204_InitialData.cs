using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class InitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "AccountType", "Balance", "CreatedAt", "Currency", "InterestRate", "Name", "Status" },
                values: new object[] { 1, "6666666666", "Savings", 500000.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", 0.15m, "Mostafa Corp", "Active" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountNumber", "AccountType", "Balance", "CreatedAt", "Currency", "Name", "OverdraftLimit", "Status" },
                values: new object[] { 2, "7777777777", "Checking", 100000.00m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "USD", "Mostafa Personal", 500m, "Active" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
