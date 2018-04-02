using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models.Annotation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using PDFAnnotation.Core.Domain.Entities;

    public class DrawingModel : BaseModel<ATDrawing, int>
    {
        public DrawingModel(IRepository<ATDrawing, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATDrawing> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATDrawing, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId)
                                                    .Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
