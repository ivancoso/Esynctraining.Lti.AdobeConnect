using System;
using System.Collections.Generic;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    public class AnnotationModel : BaseModel<ATAnnotation, int>
    {
        public AnnotationModel(IRepository<ATAnnotation, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATAnnotation> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATAnnotation, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId)
                                                    .Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }


    }
}
