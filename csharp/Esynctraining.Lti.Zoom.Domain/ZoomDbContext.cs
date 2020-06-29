using Microsoft.EntityFrameworkCore;
using Esynctraining.Lti.Zoom.Domain.Maps;

namespace Esynctraining.Lti.Zoom.Domain
{
    public class ZoomDbContext : DbContext
    {
        public ZoomDbContext(DbContextOptions<ZoomDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new LmsUserSessionMap());
            modelBuilder.ApplyConfiguration(new LmsCourseMeetingMap());
            modelBuilder.ApplyConfiguration(new OfficeHoursTeacherAvailabilityMap());
            modelBuilder.ApplyConfiguration(new OfficeHoursSlotMap());
            modelBuilder.ApplyConfiguration(new ExternalFileInfoMap());
            modelBuilder.ApplyConfiguration(new LmsMeetingSessionMap());
        }

        public DbSet<LmsCourseMeeting> LmsCourseMeetings { get; set; }
        public DbSet<OfficeHoursTeacherAvailability> OhTeacherAvailabilities { get; set; }
        public DbSet<OfficeHoursSlot> OhSlots { get; set; }
        //public DbSet<LmsUser> LmsUsers { get; set; }
        public DbSet<LmsUserSession> LmsUserSessions { get; set; }
        public DbSet<ExternalFileInfo> ExternalFiles { get; set; }
        public DbSet<LmsMeetingSession> MeetingSessions { get; set; }
        //public DbSet<LmsLicense> LmsLicenses { get; set; }
        //public DbSet<LmsLicenseSetting> LmsLicenseSettings { get; set; }
    }
}
