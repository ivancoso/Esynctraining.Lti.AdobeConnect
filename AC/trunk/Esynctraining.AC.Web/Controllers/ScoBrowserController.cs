namespace eSyncTraining.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Esynctraining.AC.Provider.DataObjects.Results;
    using Esynctraining.AC.Provider.Entities;
    using eSyncTraining.Web.Extenstions;
    using eSyncTraining.Web.Models;

    /// <summary>
    /// The sco browser controller.
    /// </summary>
    public class ScoBrowserController : AdobeConnectedControllerBase
    {
        /// <summary>
        /// The browse.
        /// </summary>
        /// <param name="scoType">
        /// The sco type.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Browse(string scoType)
        {
            ScoBrowserModel model = null;

            if (!string.IsNullOrWhiteSpace(scoType) && this.LoginWithSession().Success)
            {
                var result = this.AdobeConnect.GetContentsByType(scoType);

                model = new ScoBrowserModel
                        {
                            Values = GetContents(result),
                            ScoType = scoType,
                            FolderId = result.ScoId
                        };
            }

            return this.View(model);
        }

        public ActionResult BrowseFolder(string scoType, string folderId)
        {
            ScoBrowserModel model = null;

            if (!string.IsNullOrWhiteSpace(folderId) && this.LoginWithSession().Success)
            {
                var result = this.AdobeConnect.GetContentsByScoId(folderId);

                model = new ScoBrowserModel
                        {
                            Values = GetContents(result),
                            ScoType = scoType,
                            FolderId = result.ScoId
                        };
            }

            return this.View("Browse", model);
        }

        #region Helpers

        /// <summary>
        /// The get contents.
        /// </summary>
        /// <param name="getter">
        /// The getter.
        /// </param>
        /// <returns>
        /// Collection of Models.
        /// </returns>
        private static IEnumerable<ScoContentModel> GetContents(GenericCollectionResultBase<ScoContent> result)
        {
            return result.Success && result.Values != null
                       ? result.Values.Select(item => item.ToModel()).ToArray()
                       : Enumerable.Empty<ScoContentModel>();
        }

        #endregion

        public ActionResult Create(string scoType, string folderId)
        {
            if (scoType.EndsWith("meetings"))
            {
                return this.RedirectToAction("Create", "MeetingUpdate", new { folderId, returnUrl = this.ReferrerUrl });
            }
            else if (scoType.EndsWith("events"))
            {
                {
                    return this.RedirectToAction("Create", "EventUpdate", new { folderId, returnUrl = this.ReferrerUrl });
                }
            }

            return this.RedirectToLocal(this.ReferrerUrl);
        }

        /// <summary>
        /// The edit item.
        /// </summary>
        /// <param name="scoType">
        /// The sco Type.
        /// </param>
        /// <param name="scoId">
        /// The sco Id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult EditItem(string scoType, string scoId)
        {
            object values = null;

            if (scoType.EndsWith("meetings"))
            {
                values = new { id = scoId, returnUrl = this.ReferrerUrl };

                return this.RedirectToAction("Edit", "MeetingUpdate", values);
            }
            else if (scoType.EndsWith("events"))
            {
                values = new { id = scoId, returnUrl = this.ReferrerUrl };

                return this.RedirectToAction("Edit", "EventUpdate", values);
            }

            return this.RedirectToLocal(this.ReferrerUrl);
        }
    }
}
