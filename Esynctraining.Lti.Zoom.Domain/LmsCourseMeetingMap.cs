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
}