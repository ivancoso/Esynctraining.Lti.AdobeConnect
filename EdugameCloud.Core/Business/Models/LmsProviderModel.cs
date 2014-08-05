using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Core.Business.Models
{
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    public class LmsProviderModel : BaseModel<LmsProvider, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsProviderModel"/> class. 
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsProviderModel(IRepository<LmsProvider, int> repository)
            : base(repository)
        {
        }

        #endregion

        /// <summary>
        /// The get all active.
        /// </summary>
        /// <returns>
        /// The <see cref="LmsProvider"/>.
        /// </returns>
        public LmsProvider GetOneByName(string name)
        {
            var query = new DefaultQueryOver<LmsProvider, int>()
                .GetQueryOver()
                .WhereRestrictionOn(x => x.LmsProviderName)
                .IsInsensitiveLike(name, MatchMode.Exact)
                .Take(1);
            return this.Repository.FindOne(query).Value;
        }
    }
}
