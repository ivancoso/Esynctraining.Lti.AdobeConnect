namespace PDFAnnotation.Persistence.Mappings
{
    using PDFAnnotation.Core.Domain.Entities;

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
            this.Map(x => x.StateCode).Length(10).Not.Nullable();
            this.Map(x => x.StateName).Length(50).Nullable();
            this.Map(x => x.IsActive).Not.Nullable();
            this.References(x => x.Country).Nullable();
        }

        #endregion
    }
}