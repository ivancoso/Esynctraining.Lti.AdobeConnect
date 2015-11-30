namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;

    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;

    /// <summary>
    ///     The SN Session member model.
    /// </summary>
    public class SNMemberModel : BaseModel<SNMember, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNMemberModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNMemberModel(IRepository<SNMember, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all by session id.
        /// </summary>
        /// <param name="sessionId">
        /// The session id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNSessionMember}"/>.
        /// </returns>
        public IEnumerable<SNMember> GetAllByACSessionId(int sessionId)
        {
            var queryOver =
                new DefaultQueryOver<SNMember, int>().GetQueryOver().Where(x => x.ACSessionId == sessionId);
            return this.Repository.FindAll(queryOver);
        }

        /// <summary>
        /// The get all by session ids.
        /// </summary>
        /// <param name="sessionIds">
        /// The session ids.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNSessionMember}"/>.
        /// </returns>
        public IEnumerable<SNMember> GetAllByACSessionIds(List<int> sessionIds)
        {
            var queryOver =
                new DefaultQueryOver<SNMember, int>().GetQueryOver().WhereRestrictionOn(x => x.ACSessionId).IsIn(sessionIds);
            return this.Repository.FindAll(queryOver);
        }
    }
}