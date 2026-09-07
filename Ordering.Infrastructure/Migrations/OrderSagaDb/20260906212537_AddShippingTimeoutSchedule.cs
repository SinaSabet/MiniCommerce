using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ordering.Infrastructure.Migrations.OrderSagaDb
{
    /// <inheritdoc />
    public partial class AddShippingTimeoutSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "OrderSagas",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<Guid>(
                name: "ShippingTimeoutTokenId",
                table: "OrderSagas",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderSagas_CurrentState",
                table: "OrderSagas",
                column: "CurrentState");

            migrationBuilder.CreateIndex(
                name: "IX_OrderSagas_OrderId",
                table: "OrderSagas",
                column: "OrderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrderSagas_CurrentState",
                table: "OrderSagas");

            migrationBuilder.DropIndex(
                name: "IX_OrderSagas_OrderId",
                table: "OrderSagas");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "OrderSagas");

            migrationBuilder.DropColumn(
                name: "ShippingTimeoutTokenId",
                table: "OrderSagas");
        }
    }
}
