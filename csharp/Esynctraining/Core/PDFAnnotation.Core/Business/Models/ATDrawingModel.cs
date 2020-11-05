namespace PDFAnnotation.Core.Business.Models.Annotation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The drawing model.
    /// </summary>
    public class DrawingModel : BaseModel<ATDrawing, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public DrawingModel(IRepository<ATDrawing, int> repository)
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
