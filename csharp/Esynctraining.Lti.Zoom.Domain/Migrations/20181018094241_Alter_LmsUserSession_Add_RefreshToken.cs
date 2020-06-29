using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class Alter_LmsUserSession_Add_RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "LmsUserSession",
                maxLength: 200,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "LmsUserSession");
        }
    }
}
