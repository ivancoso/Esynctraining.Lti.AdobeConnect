using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;

    using NHibernate;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The Company model.
    /// </summary>
    public class PasswordActivationModel : BaseModel<PasswordActivation, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordActivationModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public PasswordActivationModel(IRepository<PasswordActivation, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get one by activation code.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{PasswordActivation}"/>.
        /// </returns>
        public IFutureValue<PasswordActivation> GetOneByActivationCode(Guid code)
        {
            var query =
                new DefaultQueryOver<PasswordActivation, int>().GetQueryOver()
                                                               .Where(x => x.PasswordActivationCode == code)
                                                               .Fetch(x => x.Contact).Eager
                                                               .Take(1);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get all by contact.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{PasswordActivation}"/>.
        /// </returns>
        public IEnumerable<PasswordActivation> GetAllByContact(int id)
        {
            var query = new DefaultQueryOver<PasswordActivation, int>().GetQueryOver().Where(x => x.Contact.Id == id);
            return this.Repository.FindAll(query);
        }
    }
}