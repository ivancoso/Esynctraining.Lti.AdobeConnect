namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections;
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The shape model.
    /// </summary>
    public class ShapeModel : BaseModel<ATShape, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public ShapeModel(IRepository<ATShape, int> repository)
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
        public IEnumerable<ATShape> GetAllForFile(int fileId)
        {
            var query =
                new DefaultQueryOver<ATShape, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId);
            return this.Repository.FindAll(query);
        }
    }
}
