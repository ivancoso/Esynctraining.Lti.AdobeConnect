namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The applet result mapping
    /// </summary>
    public class AppletResultMap : BaseClassMap<AppletResult>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AppletResultMap"/> class. 
        /// </summary>
        public AppletResultMap()
        {
            this.Map(x => x.ParticipantName).Length(200).Not.Nullable();
            this.Map(x => x.Score).Not.Nullable();
            this.Map(x => x.StartTime).Not.Nullable();
            this.Map(x => x.EndTime).Not.Nullable();
            this.Map(x => x.DateCreated).Not.Nullable();
            this.Map(x => x.ACSessionId).Not.Nullable();
            this.Map(x => x.IsArchive).Nullable();
            this.Map(x => x.Email).Nullable();
            this.References(x => x.AppletItem).Not.Nullable();
        }

        #endregion
    }
}