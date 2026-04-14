using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Yann_Al_Akl_WS1_TP2_Développement_Web_Serveur__1.Data.Migrations
{
    /// <inheritdoc />
    public partial class NomDeVotreMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Subjects");

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Messages");

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
