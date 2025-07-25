using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketingSystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAgencySetCategoryNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Tickets",
                type: "character varying(26)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(26)");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryId",
                table: "Tickets",
                type: "character varying(26)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(26)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Categories_CategoryId",
                table: "Tickets",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
