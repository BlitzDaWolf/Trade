using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Command.Migrations
{
    /// <inheritdoc />
    public partial class addedPnK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PnL",
                table: "Results",
                type: "REAL",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PnL",
                table: "Results");
        }
    }
}
