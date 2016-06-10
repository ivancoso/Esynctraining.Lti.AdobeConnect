using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
   // using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The user ContactType.
    /// </summary>
    public class ContactTypeModel : BaseModel<ContactType, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactTypeModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ContactTypeModel(IRepository<ContactType, int> repository)
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
        public virtual IEnumerable<ContactType> GetAllByName(string name)
        {
            var queryOver =
                new DefaultQueryOver<ContactType, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.ContactTypeName)
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
            var queryOver = new DefaultQueryOver<ContactType, int>().GetQueryOver().OrderBy(x => x.ContactTypeName).Asc.Select(x => x.ContactTypeName);
            return this.Repository.FindAll<string>(queryOver);
        }

        #endregion
    }
}