using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class LmsCourseMeetingMap : IEntityTypeConfiguration<LmsCourseMeeting>
    {
        public void Configure(EntityTypeBuilder<LmsCourseMeeting> entityBuilder)
        {
            entityBuilder.Property(x => x.CourseId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.ProviderMeetingId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.ProviderHostId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.Details).IsRequired();
        }
    }
}