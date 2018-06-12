using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Domain
{
    public class ZoomDbContext : DbContext
    {
        public ZoomDbContext(DbContextOptions<ZoomDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddConfiguration(new LmsCourseMeetingMap());
            // ...
        }

        public DbSet<LmsCourseMeeting> LmsCourseMeetings { get; set; }
        public DbSet<OfficeHoursTeacherAvailability> OhTeacherAvailabilities { get; set; }
        public DbSet<LmsUserSession> LmsUserSessions { get; set; }
        public DbSet<LmsLicense> LmsLicenses { get; set; }
        public DbSet<LmsLicenseSetting> LmsLicenseSettings { get; set; }
    }
}
