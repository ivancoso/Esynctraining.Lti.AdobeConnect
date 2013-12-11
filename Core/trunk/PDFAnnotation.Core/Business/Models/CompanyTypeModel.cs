namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The user CompanyType.
    /// </summary>
    public class CompanyTypeModel : BaseModel<CompanyType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public CompanyTypeModel(IRepository<CompanyType, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all by name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{UserRole}"/>.
        /// </returns>
        public virtual IEnumerable<CompanyType> GetAllByName(string name)
        {
            var queryOver =
                new DefaultQueryOver<CompanyType, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.CompanyTypeName)
                                                     .IsInsensitiveLike(name);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all names.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/>.
        /// </returns>
        public virtual IEnumerable<string> GetAllNames()
        {
            var queryOver = new DefaultQueryOver<CompanyType, int>().GetQueryOver().OrderBy(x => x.CompanyTypeName).Asc.Select(x => x.CompanyTypeName);
            return this.Repository.FindAll<string>(queryOver);
        }

        #endregion
    }
}