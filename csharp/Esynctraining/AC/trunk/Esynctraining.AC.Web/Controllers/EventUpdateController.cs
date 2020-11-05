namespace eSyncTraining.Web.Controllers
{
    using System.Linq;
    using System.Web.Mvc;

    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Web.Extenstions;
    using eSyncTraining.Web.Models;

    public class EventUpdateController : ScoUpdateControllerBase<EventDetailModel, EventUpdateItem>
    {
        /// <summary>
        /// Gets the view name.
        /// </summary>
        protected override string ViewName
        {
            get
            {
                return "EventUpdate";
            }
        }

        /// <summary>
        /// The get model.
        /// </summary>
        /// <param name="scoInfoResult">
        /// The sco info.
        /// </param>
        /// <returns>
        /// The <see cref="EventDetailModel"/>.
        /// </returns>
        protected override EventDetailModel GetModel(ScoInfoResult scoInfoResult)
        {
            //string permissionId = null;
            //var permission = AdobeConnect.GetScoPublicAccessPermissions(scoInfoResult.ScoInfo.ScoId);

            //if (permission.Success && permission.Values != null && permission.Values.Any())
            //{
            //    var publicAccessPermission = permission.Values.FirstOrDefault();

            //    if (publicAccessPermission != null)
            //    {
            //        permissionId = publicAccessPermission.PermissionId.ToXmlString();
            //    }
            //}

            return scoInfoResult.ScoInfo.ToEventUpdateModel();
        }

        /// <summary>
        /// The prepare view data.
        /// </summary>
        protected override void PrepareViewData()
        {
            var meetings = this.AdobeConnect.ReportAllMeetings();

            if (meetings.Success && meetings.Values != null && meetings.Values.Any())
            {
                this.ViewData["meetings"] = meetings.Values.Select(m => new SelectListItem { Value = m.ScoId, Text = string.Format("{0,10} - {1}", m.ScoId,m.MeetingName) }).ToArray();
            }
        }

        /// <summary>
        /// The model to update entity.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="EventUpdateItem"/>.
        /// </returns>
        protected override EventUpdateItem ModelToUpdateEntity(EventDetailModel model)
        {
            return model.ToUpdateItemEntity();
        }

        /// <summary>
        /// The save additional data.
        /// </summary>
        /// <param name="scoId">
        /// The sco id.
        /// </param>
        /// <param name="model">
        /// The model.
        /// </param>
        protected override void SaveAdditionalData(string scoId, EventDetailModel model)
        {
            
        }
    }
}
