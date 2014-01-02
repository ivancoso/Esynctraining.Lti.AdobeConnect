namespace PDFAnnotation.Core.Business.Models
{
    using System.Collections;
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    /// The drawing model.
    /// </summary>
    public class RotationModel : BaseModel<ATRotation, int>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RotationModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public RotationModel(IRepository<ATRotation, int> repository)
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
        public IEnumerable<ATRotation> GetAllForFile(int fileId)
        {
            var query =
                new DefaultQueryOver<ATRotation, int>().GetQueryOver()
                                                    .JoinQueryOver(x => x.Mark)
                                                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }
    }
}
