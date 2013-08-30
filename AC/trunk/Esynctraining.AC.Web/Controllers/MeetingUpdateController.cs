namespace eSyncTraining.Web.Controllers
{
    using System.Linq;

    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;
    using Esynctraining.AC.Provider.Utils;
    using eSyncTraining.Web.Extenstions;
    using eSyncTraining.Web.Models;

    public class MeetingUpdateController : ScoUpdateControllerBase<MeetingDetailModel, MeetingUpdateItem>
    {
        /// <summary>
        /// Gets the view name.
        /// </summary>
        protected override string ViewName
        {
            get
            {
                return "MeetingUpdate";
            }
        }

        /// <summary>
        /// The get model.
        /// </summary>
        /// <param name="scoInfoResult">
        /// The SCO info.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingDetailModel"/>.
        /// </returns>
        protected override MeetingDetailModel GetModel(ScoInfoResult scoInfoResult)
        {
            string permissionId = null;
            var permission = this.AdobeConnect.GetScoPublicAccessPermissions(scoInfoResult.ScoInfo.ScoId);

            if (permission.Success && permission.Values != null && permission.Values.Any())
            {
                var publicAccessPermission = permission.Values.FirstOrDefault();

                if (publicAccessPermission != null)
                {
                    permissionId = publicAccessPermission.PermissionId.ToXmlString();
                }
            }

            return scoInfoResult.ScoInfo.ToMeetingUpdateModel(permissionId);
        }

        /// <summary>
        /// The prepare view data.
        /// </summary>
        protected override void PrepareViewData()
        {
            
        }

        /// <summary>
        /// The model to update entity.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="MeetingUpdateItem"/>.
        /// </returns>
        protected override MeetingUpdateItem ModelToUpdateEntity(MeetingDetailModel model)
        {
            return model.ToUpdateItemEntity();
        }

        /// <summary>
        /// Post-Sco-update routine.
        /// Called only if sco-update succeeded!
        /// </summary>
        /// <param name="scoId">The sco id.</param>
        /// <param name="model">The model.</param>
        protected override void SaveAdditionalData(string scoId, MeetingDetailModel model)
        {
            this.AdobeConnect.UpdatePublicAccessPermissions(scoId, EnumReflector.ReflectEnum(model.PermissionId, SpecialPermissionId.remove));
        }
    }
}
