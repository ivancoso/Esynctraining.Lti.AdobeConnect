namespace EdugameCloud.Persistence.Mappings
{
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The state mapping
    /// </summary>
    public class StateMap : BaseClassMap<State>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMap"/> class.
        /// </summary>
        public StateMap()
        {
            this.Map(x => x.StateName).Length(50).Not.Nullable();
            this.Map(x => x.StateCode).Length(10).Not.Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
            this.Map(x => x.Latitude).Not.Nullable().Default("0");
            this.Map(x => x.Longitude).Not.Nullable().Default("0");
            this.Map(x => x.ZoomLevel).Not.Nullable().Default("0");
        }

        #endregion
    }
}