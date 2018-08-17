using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class ExternalFileInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalFileInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    lmsCourseMeetingId = table.Column<int>(nullable: false),
                    ProviderId = table.Column<int>(nullable: false),
                    ProviderFileRecordId = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalFileInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalFileInfo_LmsCourseMeeting_lmsCourseMeetingId",
                        column: x => x.lmsCourseMeetingId,
                        principalTable: "LmsCourseMeeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalFileInfo_lmsCourseMeetingId",
                table: "ExternalFileInfo",
                column: "lmsCourseMeetingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalFileInfo");
        }
    }
}
