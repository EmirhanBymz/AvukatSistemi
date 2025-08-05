using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AvukatSistemi.Migrations
{
    /// <inheritdoc />
    public partial class AddTCKimlikNoToClient : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TCKimlikNo",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TCKimlikNo",
                table: "Clients");
        }
    }
}
