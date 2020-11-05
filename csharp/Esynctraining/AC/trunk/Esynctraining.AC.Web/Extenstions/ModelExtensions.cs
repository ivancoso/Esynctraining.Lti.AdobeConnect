namespace eSyncTraining.Web.Extenstions
{
    using System.Globalization;

    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Web.Models;

    /// <summary>
    /// The Model extensions.
    /// </summary>
    public static class ModelExtensions
    {
        /// <summary>
        /// The to model.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="permissionId">The permission id.</param>
        /// <returns>
        /// The <see cref="ScoContentModel" />.
        /// </returns>
        public static MeetingDetailModel ToMeetingUpdateModel(this ScoInfo entity, string permissionId = null)
        {
            return new MeetingDetailModel
                       {
                           ScoId = entity.ScoId,
                           FolderId = entity.FolderId,
                           Name = entity.Name,
                           Description = entity.Description,
                           UrlPath = entity.UrlPath,
                           BeginDate = entity.BeginDate,
                           EndDate = entity.EndDate,
                           Language = entity.Language,
                           DateCreated = entity.DateCreated,
                           DateModified = entity.DateModified,
                           PermissionId = permissionId
                       };
        }

        public static EventDetailModel ToEventUpdateModel(this ScoInfo entity)
        {
            return new EventDetailModel
                       {
                           ScoId = entity.ScoId,
                           FolderId = entity.FolderId,
                           Name = entity.Name,
                           Description = entity.Description,
                           UrlPath = entity.UrlPath,
                           BeginDate = entity.BeginDate,
                           EndDate = entity.EndDate,
                           Language = entity.Language,
                           DateCreated = entity.DateCreated,
                           DateModified = entity.DateModified,
                           MeetingId = entity.SourceScoId
                       };
        }

        public static MeetingUpdateItem ToUpdateItemEntity(this MeetingDetailModel model)
        {
            return new MeetingUpdateItem
                       {
                           ScoId = model.ScoId,
                           FolderId = model.FolderId,
                           Name = model.Name,
                           Description = model.Description,
                           Language = model.Language,
                           DateBegin = model.BeginDate.ToString(CultureInfo.CurrentCulture),
                           DateEnd = model.EndDate.ToString(CultureInfo.CurrentCulture),
                           UrlPath = model.UrlPath,
                           Type = ScoType.meeting
                       };
        }

        public static EventUpdateItem ToUpdateItemEntity(this EventDetailModel model)
        {
            return new EventUpdateItem
                       {
                           ScoId = model.ScoId,
                           FolderId = model.FolderId,
                           Name = model.Name,
                           Description = model.Description,
                           Language = model.Language,
                           DateBegin = model.BeginDate.ToString(CultureInfo.CurrentCulture),
                           DateEnd = model.EndDate.ToString(CultureInfo.CurrentCulture),
                           UrlPath = model.UrlPath,
                           Type = ScoType.Event,
                           SourceScoId = model.MeetingId
                       };
        }

        public static PrincipalSlimModel ToPrincipalSlimModel(this Principal principal)
        {
            return new PrincipalSlimModel
                       {
                           PrincipalId = principal.PrincipalId,
                           Name = principal.Name,
                           Login = principal.Login,
                           HasChildren = principal.HasChildren
                       };
        }

        public static ParticipantSlimModel ToParticipantSlimModel(this PermissionInfo permission)
        {
            return new ParticipantSlimModel
                       {
                           PrincipalId = permission.PrincipalId,
                           Name = permission.Name,
                           Login = permission.Login,
                           PermissionId = permission.PermissionId
                       };
        }
    }
}