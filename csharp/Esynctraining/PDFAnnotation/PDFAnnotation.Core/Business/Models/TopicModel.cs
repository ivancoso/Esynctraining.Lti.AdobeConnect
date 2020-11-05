using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;
    using Esynctraining.Core.FullText;
    using Esynctraining.Core.Utils;

    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class TopicModel : BaseModel<Topic, int>
    {
        private readonly FullTextModel fullTextModel;


        public TopicModel(FullTextModel fullTextModel, IRepository<Topic, int> repository)
            : base(repository)
        {
            this.fullTextModel = fullTextModel;
        }


        public IEnumerable<Topic> GetAllByCategoryId(int caseId)
        {
            var defaultQuery = new DefaultQueryOver<Topic, int>().GetQueryOver().Where(x => x.Category.Id == caseId);
            return this.Repository.FindAll(defaultQuery);
        }

        public IEnumerable<Topic> GetAllByCasesIds(List<int> casesIds)
        {
            var query =
                new DefaultQueryOver<Topic, int>().GetQueryOver()
                    .WhereRestrictionOn(x => x.Category.Id)
                    .IsIn(casesIds);
            return this.Repository.FindAll(query);
        }

        public IEnumerable<Topic> GetAllByCategoryAndNamePaged(string searchPattern, int categoryId, int companyId, int pageIndex, int pageSize, out int totalCount)
        {
            var searchIds = new List<int>();
            if (pageIndex <= default(int))
            {
                pageIndex = 1;
            }

            var queryOver = new DefaultQueryOver<Topic, int>().GetQueryOver();
            if (companyId != 0)
            {
                var subquery = QueryOver.Of<Category>().Where(x => x.CompanyId == companyId).Select(x => x.Id);
                queryOver = queryOver.WithSubquery.WhereProperty(x => x.Category.Id).In(subquery);
            }

            if (categoryId != 0)
            {
                queryOver = queryOver.Where(x => x.Category.Id == categoryId);
            }

            if (!string.IsNullOrWhiteSpace(searchPattern))
            {
                searchIds = this.fullTextModel.Search(searchPattern, typeof(Topic), int.MaxValue).ToList();
                queryOver = queryOver.AndRestrictionOn(x => x.Id).IsIn(searchIds);
            }

            var rowCountQuery = queryOver.ToRowCountQuery();
            totalCount = this.Repository.FindOne<int>(rowCountQuery).Value;

            if (pageSize > 0)
            {
                var pagedQueryOver = queryOver.Take(pageSize).Skip((pageIndex - 1) * pageSize);
                return searchIds.Any() ? this.Repository.FindAll(pagedQueryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(pagedQueryOver);
            }

            return searchIds.Any() ? this.Repository.FindAll(queryOver).ToList().OrderBy(x => searchIds.IndexOf(x.Id)) : this.Repository.FindAll(queryOver);
        }

        public override void RegisterSave(Topic entity, bool flush, bool updateDateModified = true)
        {
            var fileModel = IoC.Resolve<FileModel>();
            fileModel.UpdateTopicsFiles(entity);
            base.RegisterSave(entity, flush, updateDateModified);
        }

    }

}