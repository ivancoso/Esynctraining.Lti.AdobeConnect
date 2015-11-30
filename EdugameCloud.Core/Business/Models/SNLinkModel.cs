namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;

    /// <summary>
    ///     The SN link model.
    /// </summary>
    public class SNLinkModel : BaseModel<SNLink, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNLinkModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNLinkModel(IRepository<SNLink, int> repository)
            : base(repository)
        {
        }

        #endregion
    }
}