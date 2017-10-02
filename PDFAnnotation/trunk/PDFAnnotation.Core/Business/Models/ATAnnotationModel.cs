using System;
using System.Collections.Generic;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;

namespace PDFAnnotation.Core.Business.Models
{
    public class AnnotationModel : BaseModel<ATAnnotation, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ATAnnotation"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public AnnotationModel(IRepository<ATAnnotation, int> repository)
            : base(repository)
        {
        }

        /// <summary>
        /// The get all for file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
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
