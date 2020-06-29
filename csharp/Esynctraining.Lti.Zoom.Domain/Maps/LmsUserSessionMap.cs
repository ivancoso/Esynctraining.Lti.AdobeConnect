using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Esynctraining.Lti.Zoom.Domain.Maps
{
    public class LmsUserSessionMap :  IEntityTypeConfiguration<LmsUserSession>
    {
        public void Configure(EntityTypeBuilder<LmsUserSession> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.LmsUserId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.Token).HasMaxLength(200);
            entityBuilder.Property(x => x.CourseId).HasMaxLength(200).IsRequired();
            entityBuilder.Property(x => x.SessionData).IsRequired();
        }
    }
}