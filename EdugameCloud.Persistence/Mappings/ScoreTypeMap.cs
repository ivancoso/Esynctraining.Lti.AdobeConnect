namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The score type mapping
    /// </summary>
    public class ScoreTypetMap : BaseClassMap<ScoreType>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScoreTypetMap"/> class. 
        /// </summary>
        public ScoreTypetMap()
        {
            this.Map(x => x.ScoreTypeName).Length(50).Nullable();
            this.Map(x => x.Prefix).Length(50).Nullable();
            this.Map(x => x.DefaultValue).Not.Nullable().Default("10");
            this.Map(x => x.IsActive).Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
        }

        #endregion
    }
}