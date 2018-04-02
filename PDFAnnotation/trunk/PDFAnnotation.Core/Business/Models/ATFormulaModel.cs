namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections.Generic;
    using Esynctraining.NHibernate;
    using Esynctraining.NHibernate.Queries;
    using PDFAnnotation.Core.Domain.Entities;
   public class FormulaModel : BaseModel<ATFormula, int>
    {
        public FormulaModel(IRepository<ATFormula, int> repository)
            : base(repository)
        {
        }

        public IEnumerable<ATFormula> GetAllForFile(Guid fileId)
        {
            var query =
                new DefaultQueryOver<ATFormula, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId)
                                                    .Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

    }

}
