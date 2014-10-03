namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The canvas connect credentials map
    /// </summary>
    public class CanvasConnectCredentialsMap : BaseClassMap<CanvasConnectCredentials>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CanvasConnectCredentialsMap"/> class.
        /// </summary>
        public CanvasConnectCredentialsMap()
        {
            this.Map(x => x.ACDomain).Not.Nullable();
            this.Map(x => x.CanvasDomain).Not.Nullable();
            this.Map(x => x.ACPassword).Not.Nullable();
            this.Map(x => x.ACUsername).Not.Nullable();
            this.Map(x => x.ACScoId);
            this.Map(x => x.CanvasToken);
            this.Map(x => x.ACTemplateScoId);
        }

        #endregion
    }
}
