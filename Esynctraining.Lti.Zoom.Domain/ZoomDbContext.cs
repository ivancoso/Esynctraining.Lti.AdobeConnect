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
            modelBuilder.AddConfiguration(new LmsLicenseMap());
            modelBuilder.AddConfiguration(new LmsLicenseSettingMap());
            modelBuilder.AddConfiguration(new LmsUserMap());
            modelBuilder.AddConfiguration(new LmsUserSessionMap());
            modelBuilder.AddConfiguration(new OfficeHoursTeacherAvailabilityMap());
            modelBuilder.AddConfiguration(new OfficeHoursSlotMap());
        }

        public DbSet<LmsCourseMeeting> LmsCourseMeetings { get; set; }
        public DbSet<OfficeHoursTeacherAvailability> OhTeacherAvailabilities { get; set; }
        public DbSet<OfficeHoursSlot> OhSlots { get; set; }
        public DbSet<LmsUser> LmsUsers { get; set; }
        public DbSet<LmsUserSession> LmsUserSessions { get; set; }
        public DbSet<LmsLicense> LmsLicenses { get; set; }
        public DbSet<LmsLicenseSetting> LmsLicenseSettings { get; set; }
    }
}
