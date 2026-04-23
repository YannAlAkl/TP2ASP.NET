using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Migrations
{
    /// <inheritdoc />
    public partial class DescriptionDesChangements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Categories");
        }
    }
}
