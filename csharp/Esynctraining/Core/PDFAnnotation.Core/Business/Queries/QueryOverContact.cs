namespace PDFAnnotation.Core.Business.Queries
{
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The query over user.
    /// </summary>
    public class QueryOverContact : DefaultQueryOver<Contact, int> 
    {
        #region Methods

        /// <summary>
        /// The apply.
        /// </summary>
        /// <param name="queryOver">
        /// The query over.
        /// </param>
        /// <returns>
        /// The <see cref="QueryOver"/>.
        /// </returns>
        protected override QueryOver<Contact, Contact> Apply(QueryOver<Contact, Contact> queryOver)
        {
            var deleted = ContactStatusEnum.Deleted;
            return base.Apply(queryOver).Where(x => x.Status != deleted);
        }

        #endregion
    }
}