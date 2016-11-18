namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Utils;

    /// <summary>
    /// The distractor item mapping
    /// </summary>
    public class DistractorMap : BaseClassMap<Distractor>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DistractorMap"/> class. 
        /// </summary>
        public DistractorMap()
        {
            this.Map(x => x.DistractorName).Not.Nullable();
            this.Map(x => x.DistractorOrder).Not.Nullable();
            this.Map(x => x.Score).Length(50).Nullable();
            this.Map(x => x.IsCorrect).Nullable();
            this.Map(x => x.IsActive).Nullable();
            this.Map(x => x.DistractorType).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.DateModified).Not.Nullable();
            this.Map(x => x.LmsAnswer).Length(100).Nullable();
            this.Map(x => x.LmsAnswerId).Nullable();
            this.Map(x => x.LmsProviderId).Nullable();

            this.References(x => x.Question).Nullable();
            this.References(x => x.Image).Column("imageId").Nullable();
            this.References(x => x.LeftImage).Column("leftImageId").Nullable();
            this.References(x => x.RightImage).Column("rightImageId").Nullable();
            this.References(x => x.CreatedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Distractor>(x => x.CreatedBy)));
            this.References(x => x.ModifiedBy).Nullable().LazyLoad().Column(Inflector.Uncapitalize(Lambda.Property<Distractor>(x => x.ModifiedBy)));
        }

        #endregion
    }
}