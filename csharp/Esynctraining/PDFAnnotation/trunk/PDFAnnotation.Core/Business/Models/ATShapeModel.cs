using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using PDFAnnotation.Core.Domain.Entities;

    public class ShapeModel : BaseModel<ATShape, int>
    {
        public ShapeModel(IRepository<ATShape, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATShape> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATShape, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
