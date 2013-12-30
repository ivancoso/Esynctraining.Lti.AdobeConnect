namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The SN map settings map.
    /// </summary>
    public class SNMapSettingsMap : BaseClassMap<SNMapSettings>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMapSettingsMap"/> class.
        /// </summary>
        public SNMapSettingsMap()
        {
            this.References(x => x.MapProvider).Nullable();
            this.References(x => x.Country).Nullable();
            this.Map(x => x.ZoomLevel).Nullable();
        }

        #endregion
    }
}