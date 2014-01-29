namespace PDFAnnotation.Core.Business.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate.Criterion;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The TextItem model class.
    /// </summary>
    public class TextItemModel : BaseModel<ATTextItem, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextItemModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public TextItemModel(IRepository<ATTextItem, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get all for file.
        /// </summary>
        /// <param name="fileId">
        /// The file id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<ATTextItem> GetAllForFile(Guid fileId)
        {
            QueryOver<ATTextItem, ATMark> query =
                new DefaultQueryOver<ATTextItem, int>().GetQueryOver()
                    .JoinQueryOver(x => x.Mark)
                    .Where(x => x.File.Id == fileId).Fetch(x => x.Mark).Eager;
            return this.Repository.FindAll(query);
        }

        #endregion
    }
}