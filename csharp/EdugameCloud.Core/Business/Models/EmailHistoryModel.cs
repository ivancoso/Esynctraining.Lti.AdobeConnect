namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using NHibernate;

    public class EmailHistoryModel : BaseModel<EmailHistory, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailHistoryModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public EmailHistoryModel(IRepository<EmailHistory, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{EmailHistory}"/>.
        /// </returns>
        public override IEnumerable<EmailHistory> GetAll()
        {
            var query = new DefaultQueryOver<EmailHistory, int>().GetQueryOver().OrderBy(x => x.Date).Asc;
            return this.Repository.FindAll(query);
        }

        /// <summary>
        /// The get one by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{EmailHistory}"/>.
        /// </returns>
        public override IFutureValue<EmailHistory> GetOneById(int id)
        {
            var queryOver = new DefaultQueryOver<EmailHistory, int>().GetQueryOver().Where(x => x.Id == id);
            return this.Repository.FindOne(queryOver);
        }
    }
}
