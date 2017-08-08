using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Core.Domain.Entities;
using Esynctraining.Core.Caching;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate.Linq;

namespace EdugameCloud.Core.Business.Models
{
    public class CompanyEventQuizMappingModel : BaseModel<CompanyEventQuizMapping, int>
    {
        private ICache _cache;


        public CompanyEventQuizMappingModel(IRepository<CompanyEventQuizMapping, int> repository, ICache cache)
            : base(repository)
        {
            _cache = cache;
        }


        public IEnumerable<CompanyEventQuizMapping> GetAllByAcServerId(int companyAcServerId)
        {
            var eventMappings = GetAll();
            var items = eventMappings.Where(x => x.CompanyAcDomain.Id == companyAcServerId);
            return items;
        }

        public IEnumerable<CompanyEventQuizMapping> GetAllByCompanyId(int companyId)
        {
            var eventMappings = GetAll();
            var items = eventMappings.Where(x => x.CompanyAcDomain.Company.Id == companyId);
            return items;
        }

        public CompanyEventQuizMapping GetByGuid(Guid guid)
        {
            return GetAll(x => x.Guid == guid).FirstOrDefault();
        }

        public bool AnyByQuizId(int quizId)
        {
            var query = from map in this.Repository.Session.Query<CompanyEventQuizMapping>()
                        where map.PreQuiz.Id == quizId || map.PostQuiz.Id == quizId
                        select map;
            return query.Any();
        }

    }

}