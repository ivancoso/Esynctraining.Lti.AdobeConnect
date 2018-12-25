using Microsoft.EntityFrameworkCore.Migrations;

namespace Esynctraining.Lti.Zoom.Domain.Migrations
{
    public partial class Add_LmsCalendarEventId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LmsCalendarEventId",
                table: "LmsMeetingSession",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LmsCalendarEventId",
                table: "LmsCourseMeeting",
                maxLength: 50,
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
