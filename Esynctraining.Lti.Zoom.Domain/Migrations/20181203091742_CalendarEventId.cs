using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class CalendarEventId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LmsCalendarEventId",
                table: "LmsMeetingSession",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LmsCalendarEventId",
                table: "LmsCourseMeeting",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LmsCalendarEventId",
                table: "LmsMeetingSession");

            migrationBuilder.DropColumn(
                name: "LmsCalendarEventId",
                table: "LmsCourseMeeting");
        }
    }
}
