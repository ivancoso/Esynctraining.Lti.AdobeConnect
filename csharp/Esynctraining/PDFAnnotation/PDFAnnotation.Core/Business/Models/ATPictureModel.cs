namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using PDFAnnotation.Core.Domain.Entities;
    public class PictureModel : BaseModel<ATPicture, int>
    {
        public PictureModel(IRepository<ATPicture, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATPicture> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATPicture, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId)
                                                    .Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
