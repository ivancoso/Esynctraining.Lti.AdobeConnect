namespace EdugameCloud.Lti.Persistence.Mappings
{
    using EdugameCloud.Lti.Domain.Entities;

    using Esynctraining.Persistence.Mappings;

    /// <summary>
    /// The LMS Provider item mapping
    /// </summary>
    public class LmsProviderMap : BaseClassMap<LmsProvider>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsProviderMap"/> class. 
        /// </summary>
        public LmsProviderMap()
        {
            this.Map(x => x.LmsProviderName).Nullable();
            this.Map(x => x.ShortName).Nullable();
            this.Map(x => x.ConfigurationUrl).Nullable();
            this.Map(x => x.UserGuideFileUrl).Nullable();
        }

        #endregion
    }
}
