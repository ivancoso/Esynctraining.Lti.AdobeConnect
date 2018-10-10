using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class LmsMeetingSessionMap : IEntityTypeConfiguration<LmsMeetingSession>
    {
        public void Configure(EntityTypeBuilder<LmsMeetingSession> entityBuilder)
        {
            entityBuilder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.Summary).HasMaxLength(2000);
            entityBuilder.HasOne(x => x.Meeting).WithMany(x => x.MeetingSessions).HasForeignKey("lmsCourseMeetingId").IsRequired().OnDelete(DeleteBehavior.Cascade);
        }
    }
}