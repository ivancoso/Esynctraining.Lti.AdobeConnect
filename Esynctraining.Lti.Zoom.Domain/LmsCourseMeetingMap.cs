using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain
{
    public class LmsCourseMeetingMap : DbEntityConfiguration<LmsCourseMeeting>
    {
        public override void Configure(EntityTypeBuilder<LmsCourseMeeting> entityBuilder)
        {
            // entityBuilder.Property(x => x.PrincipalId).HasMaxLength(100);
        }
    }

    public class LmsLicenseMap : DbEntityConfiguration<LmsLicense>
    {
        public override void Configure(EntityTypeBuilder<LmsLicense> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }

    public class LmsLicenseSettingMap : DbEntityConfiguration<LmsLicenseSetting>
    {
        public override void Configure(EntityTypeBuilder<LmsLicenseSetting> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.HasOne(x => x.License).WithMany(x => x.Settings).HasForeignKey("lmsLicenseId");
        }
    }

    public class LmsUserSessionMap : DbEntityConfiguration<LmsUserSession>
    {
        public override void Configure(EntityTypeBuilder<LmsUserSession> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }

    public class LmsUserMap : DbEntityConfiguration<LmsUser>
    {
        public override void Configure(EntityTypeBuilder<LmsUser> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }

    public class OfficeHoursTeacherAvailabilityMap : DbEntityConfiguration<OfficeHoursTeacherAvailability>
    {
        public override void Configure(EntityTypeBuilder<OfficeHoursTeacherAvailability> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
        }
    }
}