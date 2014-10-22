namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The LMS user parameters map.
    /// </summary>
    public class LmsUserParametersMap : BaseClassMap<LmsUserParameters>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersMap"/> class.
        /// </summary>
        public LmsUserParametersMap()
        {
            this.Map(x => x.AcId).Length(10).Not.Nullable();
            this.Map(x => x.Course).Not.Nullable();
            this.Map(x => x.Domain).Length(50).Not.Nullable();
            this.Map(x => x.Provider).Length(50).Not.Nullable();
            this.Map(x => x.Wstoken).Length(50).Not.Nullable();

            this.References(x => x.LmsUser).Nullable();
        }

        #endregion
    }
}
