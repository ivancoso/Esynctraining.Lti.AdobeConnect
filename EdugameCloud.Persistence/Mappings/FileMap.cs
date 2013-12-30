namespace EdugameCloud.Persistence.Mappings
{
    using System.Data.SqlTypes;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Enums;

    /// <summary>
    /// The user map.
    /// </summary>
    public class FileMap : BaseClassMap<File>
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
            this.Map(x => x.WebOrbId).Nullable();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column("createdBy");
        }


        #endregion
    }
}