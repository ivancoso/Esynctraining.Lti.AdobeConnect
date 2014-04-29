namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The user map.
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
            this.Map(x => x.Height).Nullable();
            this.Map(x => x.Width).Nullable();
            this.Map(x => x.X).Nullable();
            this.Map(x => x.Y).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.Status).Nullable().CustomType<ImageStatus>();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column("createdBy");
            this.HasMany(x => x.Distractors).ExtraLazyLoad().KeyColumn("imageId");
            this.HasMany(x => x.Questions).ExtraLazyLoad().KeyColumn("imageId");
        }


        #endregion
    }
}