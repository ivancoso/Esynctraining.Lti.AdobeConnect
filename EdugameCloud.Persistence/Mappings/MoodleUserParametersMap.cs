namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    public class MoodleUserParametersMap : BaseClassMap<MoodleUserParameters>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserParametersMap"/> class.
        /// </summary>
        public MoodleUserParametersMap()
        {
            this.Map(x => x.AcId).Length(10).Not.Nullable();
            this.Map(x => x.Course).Not.Nullable();
            this.Map(x => x.Domain).Length(50).Not.Nullable();
            this.Map(x => x.Provider).Length(50).Not.Nullable();
            this.Map(x => x.Wstoken).Length(50).Not.Nullable();
        }

        #endregion
    }
}
