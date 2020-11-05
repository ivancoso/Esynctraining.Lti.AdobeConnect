namespace eSyncTraining.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Utils;
    using eSyncTraining.Web.Extenstions;
    using eSyncTraining.Web.Models;

    public class MeetingParticipantsController : AdobeConnectedControllerBase
    {
        /// <summary>
        /// The view name.
        /// </summary>
        private const string ViewName = "MeetingParticipants";

        //
        // GET: /MeetingParticipants/

        /// <summary>
        /// The index.
        /// </summary>
        /// <param name="meetingId">The meeting id.</param>
        /// <param name="meetingName">Name of the meeting.</param>
        /// <param name="currentPrincipalId">The current principal id.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        public ActionResult Index(string meetingId, string meetingName, string currentPrincipalId, string returnUrl)
        {
            ViewBag.ReturnUrl = string.IsNullOrWhiteSpace(returnUrl) ? this.ReferrerUrl : returnUrl;
            ViewBag.CurrentPrincipalId = currentPrincipalId;
            ViewBag.MeetingId = meetingId;
            ViewBag.MeetingName = meetingName;


            if (!this.LoginWithSession().Success)
            {
                return this.View(ViewName);
            }

            var principalsResult = AdobeConnect.GetGroupPrincipalUsers(currentPrincipalId);

            if (!principalsResult.Success || !principalsResult.Values.Any())
            {
                return this.View(ViewName);
            }

            var hosts = AdobeConnect.GetMeetingHosts(meetingId).Values ?? Enumerable.Empty<PermissionInfo>();
            var presenters = AdobeConnect.GetMeetingPresenters(meetingId).Values ?? Enumerable.Empty<PermissionInfo>();
            var participants = AdobeConnect.GetMeetingParticipants(meetingId).Values ?? Enumerable.Empty<PermissionInfo>();

            var allParticipantIds = new List<string>();

            allParticipantIds.AddRange(hosts.Select(p => p.PrincipalId));
            allParticipantIds.AddRange(presenters.Select(p => p.PrincipalId));
            allParticipantIds.AddRange(participants.Select(p => p.PrincipalId));
            
            var model = new MeetingParticipantsModel
                            {
                                Principals = principalsResult.Values.Where(p => !allParticipantIds.Any(id => id.Equals(p.PrincipalId, StringComparison.InvariantCultureIgnoreCase))).Select(p => p.ToPrincipalSlimModel()).ToArray(),
                                Hosts = hosts.Select(p => p.ToParticipantSlimModel()).ToArray(),
                                Presenters = presenters.Select(p => p.ToParticipantSlimModel()).ToArray(),
                                Participants = participants.Select(p => p.ToParticipantSlimModel()).ToArray()
                            };

            return this.View(ViewName, model);
        }

        /// <summary>
        /// The set permission.
        /// </summary>
        /// <param name="meetingId">The meeting id.</param>
        /// <param name="meetingName">Name of the meeting.</param>
        /// <param name="principalId">The principal id.</param>
        /// <param name="permissionId">The permission id.</param>
        /// <param name="currentPrincipalId">The current principal id.</param>
        /// <param name="returnUrl">The return url.</param>
        /// <returns>
        /// The <see cref="ActionResult" />.
        /// </returns>
        [HttpPost]
        public ActionResult SetPermission(string meetingId, string meetingName, string principalId, string permissionId, string currentPrincipalId, string returnUrl)
        {
            try
            {
                if (this.LoginWithSession().Success)
                {
                    AdobeConnect.UpdateMeetingPermissionForPrincipal(meetingId, principalId, EnumReflector.ReflectEnum(permissionId, MeetingPermissionId.not_set));
                }

                return RedirectToAction("Index", new { meetingId, meetingName, currentPrincipalId, returnUrl });
            }
            catch
            {
            }

            return View(ViewName);
        }
    }
}
