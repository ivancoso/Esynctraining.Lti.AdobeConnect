namespace Esynctraining.Core.FullText
{
    using System.Diagnostics.CodeAnalysis;

    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Utils;
    using NHibernate.Event;

    /// <summary>
    /// The lucene ft index event listener.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    // ReSharper disable once InconsistentNaming
    public class LuceneFTIndexEventListener : IPostUpdateEventListener,
                                              IPostDeleteEventListener,
                                              IPostInsertEventListener
    {
        #region Fields

        /// <summary>
        ///     The full text model.
        /// </summary>
        private readonly FullTextModel fullTextModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LuceneFTIndexEventListener" /> class.
        /// </summary>
        public LuceneFTIndexEventListener()
        {
            this.fullTextModel = IoC.Resolve<FullTextModel>();
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Removes Full-Text Indices on deleted entities.
        /// </summary>
        /// <param name="e">
        /// Post delete parameters
        /// </param>
        public void OnPostDelete(PostDeleteEvent e)
        {
            this.fullTextModel.DeleteIndexForEntity(e.Entity);
        }

        /// <summary>
        /// Adds a Full-Text index on the entity.
        /// </summary>
        /// <param name="e">
        /// Post insert parameters
        /// </param>
        public void OnPostInsert(PostInsertEvent e)
        {
            this.fullTextModel.InsertIndexOnEntity(e.Entity);
        }

        /// <summary>
        /// Updates the Full-Text index on the entity.
        /// </summary>
        /// <param name="e">
        /// Post update parameters
        /// </param>
        public void OnPostUpdate(PostUpdateEvent e)
        {
            this.fullTextModel.UpdateIndexOnEntity(e.Entity);
        }

        #endregion
    }
}
