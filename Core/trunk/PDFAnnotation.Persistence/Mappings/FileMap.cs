namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Core.Enums;
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The image map.
    /// </summary>
    public class FileMap : BaseClassMapGuid<File>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileMap"/> class.
        /// </summary>
        public FileMap()
        {
            this.Map(x => x.FileName).Length(255).Not.Nullable();
            this.Map(x => x.FileSize).Length(255).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.Status).Nullable().CustomType<FileStatus>();
            this.Map(x => x.Description).Nullable();
            this.Map(x => x.TopicName).Column("topicName").Length(255).Nullable();
            this.Map(x => x.DisplayName).Length(255).Nullable();
            this.Map(x => x.DateModified).Nullable();
            this.Map(x => x.FileNumber).Nullable();
            this.Map(x => x.IsShared).Nullable();
            this.Map(x => x.IsOriginal).Nullable();
            this.Map(x => x.AcMeetingUrl).Column("acMeetingUrl").Length(1000).Nullable();

            this.References(x => x.User).Column("userId").Nullable();
            this.References(x => x.Topic).Column("topicId").Nullable();
            this.References(x => x.Category).Column("categoryId").Nullable();

            this.HasMany(x => x.Marks).Cascade.Delete().Inverse().ExtraLazyLoad();
        }

        #endregion
    }
}