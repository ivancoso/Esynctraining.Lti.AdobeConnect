using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class OfficeHoursSlotMap : IEntityTypeConfiguration<OfficeHoursSlot>
    {
        public void Configure(EntityTypeBuilder<OfficeHoursSlot> entityBuilder)
        {
            entityBuilder.HasOne(x => x.Availability).WithMany(x => x.Slots).HasForeignKey("availabilityId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.Property(x => x.LmsUserId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.RequesterName).HasMaxLength(200);
            entityBuilder.Property(x => x.Subject).HasMaxLength(200);
            entityBuilder.Property(x => x.Questions).HasMaxLength(2000);
        }
    }
}