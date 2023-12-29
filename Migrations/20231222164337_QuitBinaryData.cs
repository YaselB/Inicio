using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace inicio.Migrations
{
    /// <inheritdoc />
    public partial class QuitBinaryData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Fotos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Image",
                table: "Fotos",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
