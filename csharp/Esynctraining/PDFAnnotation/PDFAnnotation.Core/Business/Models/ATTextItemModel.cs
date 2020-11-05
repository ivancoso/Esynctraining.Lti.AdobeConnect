using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using NHibernate.Criterion;
    using PDFAnnotation.Core.Domain.Entities;

    public class TextItemModel : BaseModel<ATTextItem, int>
    {
        public TextItemModel(IRepository<ATTextItem, int> repository)
            : base(repository)
        {
        }


        public IEnumerable<ATTextItem> GetAllForFile(Guid fileId)
        {
            QueryOver<ATTextItem, ATMark> query =
                new DefaultQueryOver<ATTextItem, int>().GetQueryOver()
                    .JoinQueryOver(x => x.Mark)
                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}