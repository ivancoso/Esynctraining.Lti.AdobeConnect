using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class ExternalFileInfoMap : IEntityTypeConfiguration<ExternalFileInfo>
    {
        public void Configure(EntityTypeBuilder<ExternalFileInfo> entityBuilder)
        {
            entityBuilder.HasOne(x => x.Meeting).WithMany().HasForeignKey("lmsCourseMeetingId").IsRequired().OnDelete(DeleteBehavior.Cascade);
            entityBuilder.Property(x => x.ProviderFileRecordId).HasMaxLength(200).IsRequired();
        }
    }
}