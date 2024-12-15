using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Labb2DatabasTest.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyBokLagerSaldo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Adding the foreign key constraint from LagerSaldo to Bok
            migrationBuilder.AddForeignKey(
                name: "FK_LagerSaldo_Böcker",
                table: "LagerSaldo",
                column: "ISBN13",
                principalTable: "Böcker",
                principalColumn: "ISBN13",
                onDelete: ReferentialAction.Restrict); // Or "Cascade" depending on your requirement
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop only the foreign key constraints
            migrationBuilder.DropForeignKey(
                name: "FK_LagerSaldo_Böcker_ISBN13",
                table: "LagerSaldo");

            migrationBuilder.DropForeignKey(
                name: "FK_LagerSaldo_Butiker",
                table: "LagerSaldo");

            // Optionally, drop indexes or any other constraints if necessary
            migrationBuilder.DropIndex(
                name: "IX_LagerSaldo_ISBN13",
                table: "LagerSaldo");

            // You can keep the tables and data intact
        }
    }
}