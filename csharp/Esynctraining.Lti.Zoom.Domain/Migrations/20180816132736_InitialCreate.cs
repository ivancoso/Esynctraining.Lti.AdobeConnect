using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LmsCourseMeeting",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LicenseKey = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    CourseId = table.Column<string>(maxLength: 200, nullable: false),
                    ProviderMeetingId = table.Column<string>(maxLength: 200, nullable: false),
                    ProviderHostId = table.Column<string>(maxLength: 200, nullable: false),
                    Reused = table.Column<bool>(nullable: false),
                    Details = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LmsCourseMeeting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LmsUserSession",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    LicenseKey = table.Column<Guid>(nullable: false),
                    Email = table.Column<string>(maxLength: 200, nullable: false),
                    LmsUserId = table.Column<string>(maxLength: 200, nullable: false),
                    Token = table.Column<string>(maxLength: 200, nullable: true),
                    CourseId = table.Column<string>(maxLength: 200, nullable: false),
                    SessionData = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LmsUserSession", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OfficeHoursTeacherAvailability",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LmsUserId = table.Column<string>(maxLength: 200, nullable: false),
                    Duration = table.Column<int>(nullable: false),
                    Intervals = table.Column<string>(maxLength: 1000, nullable: false),
                    DaysOfWeek = table.Column<string>(maxLength: 20, nullable: false),
                    PeriodStart = table.Column<DateTime>(nullable: false),
                    PeriodEnd = table.Column<DateTime>(nullable: false),
                    lmsCourseMeetingId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeHoursTeacherAvailability", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeHoursTeacherAvailability_LmsCourseMeeting_lmsCourseMeetingId",
                        column: x => x.lmsCourseMeetingId,
                        principalTable: "LmsCourseMeeting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OfficeHoursSlot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    availabilityId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    LmsUserId = table.Column<string>(maxLength: 200, nullable: false),
                    RequesterName = table.Column<string>(maxLength: 200, nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false),
                    Subject = table.Column<string>(maxLength: 200, nullable: true),
                    Questions = table.Column<string>(maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OfficeHoursSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OfficeHoursSlot_OfficeHoursTeacherAvailability_availabilityId",
                        column: x => x.availabilityId,
                        principalTable: "OfficeHoursTeacherAvailability",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OfficeHoursSlot_availabilityId",
                table: "OfficeHoursSlot",
                column: "availabilityId");

            migrationBuilder.CreateIndex(
                name: "IX_OfficeHoursTeacherAvailability_lmsCourseMeetingId",
                table: "OfficeHoursTeacherAvailability",
                column: "lmsCourseMeetingId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LmsUserSession");

            migrationBuilder.DropTable(
                name: "OfficeHoursSlot");

            migrationBuilder.DropTable(
                name: "OfficeHoursTeacherAvailability");

            migrationBuilder.DropTable(
                name: "LmsCourseMeeting");
        }
    }
}
