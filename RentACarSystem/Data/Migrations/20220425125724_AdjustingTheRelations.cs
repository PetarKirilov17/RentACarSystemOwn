using Microsoft.EntityFrameworkCore.Migrations;

namespace RentACarSystem.Data.Migrations
{
    public partial class AdjustingTheRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queries_Cars_CarId",
                table: "Queries");

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Queries",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_Cars_CarId",
                table: "Queries",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Queries_Cars_CarId",
                table: "Queries");

            migrationBuilder.AlterColumn<int>(
                name: "CarId",
                table: "Queries",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Queries_Cars_CarId",
                table: "Queries",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
