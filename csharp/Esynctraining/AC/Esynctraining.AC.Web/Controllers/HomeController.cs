namespace eSyncTraining.Web.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using System.Linq;

    using eSyncTraining.Web.Extenstions;
    using eSyncTraining.Web.Models;

    public class HomeController : AdobeConnectedControllerBase
    {
        #region Helpers

        private UserInfoModel GetUserInfo()
        {
            var model = new UserInfoModel
            {
                Username = "N/A"
            };

            var result = this.LoginWithSession();

            if (result.Success)
            {
                model.Username = result.User.Name;
                model.Login = result.User.Login;
            }

            return model;
        }

        #endregion

        public ActionResult Index()
        {
            this.ViewBag.Message = "Welcome!";

            return this.View();
        }

        public ActionResult UserInfo(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            return this.View(this.GetUserInfo());
        }

        public ActionResult MyEvents(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            var events = new List<EventModel>();

            this.LoginWithSession();

            var result = this.AdobeConnect.ReportMyEvents();

            if (result.Success)
            {
                events.AddRange(result.Values.Select(eventInfo => new EventModel
                    {
                        ScoId = eventInfo.ScoId,
                        TreeId = eventInfo.TreeId,
                        Name = eventInfo.Name,
                        DomainName = eventInfo.DomainName,
                        Path = eventInfo.UrlPath,
                        BeginDate = eventInfo.DateBegin,
                        EndDate = eventInfo.DateEnd,
                        IsExpired = eventInfo.Expired,
                        Duration = eventInfo.Duration
                    }));
            }

            return this.View(events.ToArray());
        }

        public ActionResult AllEvents(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            var events = new List<ScoContentModel>();

            this.LoginWithSession();

            var result = this.AdobeConnect.GetAllEvents();

            if (result.Success)
            {
                events.AddRange(result.Values.Select(eventInfo => eventInfo.ToModel()));
            }

            return this.View(events.ToArray());
        }

        public ActionResult AllMeetings(string returnUrl)
        {
            this.ViewBag.ReturnUrl = returnUrl;

            var meetings = new List<MeetingItemModel>();

            this.LoginWithSession();

            var result = this.AdobeConnect.ReportAllMeetings();

            if (result.Success)
            {
                meetings.AddRange(result.Values.Select(meetingItem => new MeetingItemModel
                    {
                        ScoId = meetingItem.ScoId,
                        FolderId = meetingItem.FolderId,
                        Name = meetingItem.MeetingName,
                        DomainName = meetingItem.DomainName,
                        Path = meetingItem.UrlPath,
                        ActiveParticipants = meetingItem.ActiveParticipants,
                        BeginDate = meetingItem.DateBegin,
                        EndDate = meetingItem.DateEnd,
                        IsExpired = meetingItem.Expired,
                        Duration = meetingItem.Duration
                    }));
            }

            return this.View(meetings.ToArray());
        }
    }
}
