using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using PDFAnnotation.Core.Domain.Entities;

    public class RotationModel : BaseModel<ATRotation, int>
    {
        public RotationModel(IRepository<ATRotation, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATRotation> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATRotation, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
