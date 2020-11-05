namespace PDFAnnotation.Core.Business.Models
{
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The state model.
    /// </summary>
    public class StateModel : BaseModel<State, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public StateModel(IRepository<State, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}