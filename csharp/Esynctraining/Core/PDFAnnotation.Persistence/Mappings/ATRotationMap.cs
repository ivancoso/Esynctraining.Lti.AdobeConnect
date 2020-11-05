namespace PDFAnnotation.Persistence.Mappings
{
    using Esynctraining.Persistence.Mappings;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The drawing map.
    /// </summary>
    public class ATRotationMap : BaseClassMap<ATRotation>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ATRotationMap"/> class.
        /// </summary>
        public ATRotationMap()
        {
            this.References(x => x.Mark).Not.Nullable();
        }

        #endregion
    }
}