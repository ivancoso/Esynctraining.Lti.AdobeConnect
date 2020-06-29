using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class OfficeHoursTeacherAvailabilityMap : IEntityTypeConfiguration<OfficeHoursTeacherAvailability>
    {
        public void Configure(EntityTypeBuilder<OfficeHoursTeacherAvailability> entityBuilder)
        {
            entityBuilder.HasOne(x => x.Meeting).WithMany().HasForeignKey("lmsCourseMeetingId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.Property(x => x.LmsUserId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.Intervals).HasMaxLength(1000).IsRequired();
            entityBuilder.Property(x => x.DaysOfWeek).HasMaxLength(20).IsRequired();
        }
    }
}