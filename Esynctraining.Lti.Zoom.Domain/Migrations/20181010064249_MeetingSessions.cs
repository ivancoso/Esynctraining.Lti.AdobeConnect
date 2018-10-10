using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class MeetingSessions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LmsMeetingSession",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: false),
                    lmsCourseMeetingId = table.Column<int>(nullable: false),
                    Summary = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LmsMeetingSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LmsMeetingSession_LmsCourseMeeting_lmsCourseMeetingId",
                        column: x => x.lmsCourseMeetingId,
                        principalTable: "LmsCourseMeeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LmsMeetingSession_lmsCourseMeetingId",
                table: "LmsMeetingSession",
                column: "lmsCourseMeetingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LmsMeetingSession");
        }
    }
}
