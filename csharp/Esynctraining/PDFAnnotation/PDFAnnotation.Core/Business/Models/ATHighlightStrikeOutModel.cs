using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;

    using PDFAnnotation.Core.Domain.Entities;

    public class HighlightStrikeOutModel : BaseModel<ATHighlightStrikeOut, int>
    {
        public HighlightStrikeOutModel(IRepository<ATHighlightStrikeOut, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATHighlightStrikeOut> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATHighlightStrikeOut, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
