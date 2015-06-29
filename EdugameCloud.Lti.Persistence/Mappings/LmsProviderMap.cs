using Esynctraining.Persistence.Mappings;
using EdugameCloud.Lti.Domain.Entities;

namespace EdugameCloud.Lti.Persistence.Mappings
{
    /// <summary>
    /// The LMS Provider item mapping
    /// </summary>
    public sealed class LmsProviderMap : BaseClassMap<LmsProvider>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsProviderMap"/> class. 
        /// </summary>
        public LmsProviderMap()
        {
            this.Map(x => x.LmsProviderName).Not.Nullable();
            this.Map(x => x.ShortName).Not.Nullable();
            this.Map(x => x.ConfigurationUrl).Nullable();
            this.Map(x => x.UserGuideFileUrl).Nullable();
        }

        #endregion
    }
}
