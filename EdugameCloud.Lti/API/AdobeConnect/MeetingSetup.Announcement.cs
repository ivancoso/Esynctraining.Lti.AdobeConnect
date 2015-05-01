using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.API.Canvas;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed partial class MeetingSetup
    {
        #region Properties

        private IBlackBoardApi BlackboardApi
        {
            get
            {
                return IoC.Resolve<IBlackBoardApi>();
            }
        }

        private ICanvasAPI CanvasApi
        {
            get
            {
                return IoC.Resolve<ICanvasAPI>();
            }
        }
        
        #endregion

        private void CreateAnnouncement(
            LmsCompany lmsCompany, 
            LtiParamDTO param, 
            string name, 
            string startDate, 
            string startTime, 
            string duration)
        {
            if (!lmsCompany.ShowAnnouncements.GetValueOrDefault() || string.IsNullOrEmpty(param.context_title))
            {
                return;
            }

            var announcementTitle = string.Format(
                "A new Adobe Connect room was created for course {0}",
                param.context_title);
            const string AnnouncementMessagePattern = "Meeting \"{0}\" will start {1} at {2}. Its duration will be {3}. You can join it in your <a href='{4}'>Adobe Connect Conference section</a>.";
            var announcementMessage = string.Format(
                AnnouncementMessagePattern,
                name,
                startDate,
                startTime,
                duration,
                param.referer ?? string.Empty);

            switch (lmsCompany.LmsProvider.ShortName.ToLowerInvariant())
            {
                case LmsProviderNames.Canvas:
                    var lmsUser = LmsUserModel.GetOneByUserIdAndCompanyLms(param.lms_user_id, lmsCompany.Id).Value;
                    var token = lmsUser.Return(
                            u => u.Token,
                            lmsCompany.AdminUser.Return(a => a.Token, string.Empty));
                    CanvasApi.CreateAnnouncement(
                        lmsCompany.LmsDomain,
                        token,
                        param.course_id,
                        announcementTitle,
                        announcementMessage);
                    break;
                case LmsProviderNames.Blackboard:
                    BlackboardApi.CreateAnnouncement(param.course_id, lmsCompany, announcementTitle, announcementMessage);
                    break;
                case LmsProviderNames.BrainHoney:
                    // string error;
//                    this.dlapApi.CreateAnnouncement(
//                        credentials,
//                        param.course_id,
//                        announcementTitle,
//                        announcementMessage, 
//                        out error);
                    break;
            }
        }

    }
}