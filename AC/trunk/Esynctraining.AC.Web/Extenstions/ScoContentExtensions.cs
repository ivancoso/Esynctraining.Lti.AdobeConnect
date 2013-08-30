namespace eSyncTraining.Web.Extenstions
{
    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Web.Models;

    /// <summary>
    /// The sco content extensions.
    /// </summary>
    public static class ScoContentExtensions
    {
        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ScoContentModel"/>.
        /// </returns>
        public static ScoContentModel ToModel(this ScoContent entity)
        {
            return new ScoContentModel
                       {
                           ScoId = entity.ScoId,
                           SourceScoId = entity.SourceScoId,
                           FolderId = entity.FolderId,
                           Type = entity.Type,
                           Name = entity.Name,
                           Description = entity.Description,
                           Path = entity.UrlPath,
                           BeginDate = entity.BeginDate,
                           EndDate = entity.EndDate,
                           DateCreated = entity.DateCreated,
                           DateModified = entity.DateModified,
                           Duration = entity.Duration
                       };
        }
    }
}