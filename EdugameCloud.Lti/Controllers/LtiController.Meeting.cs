//using Castle.Core.Logging;
//using EdugameCloud.Core.Business.Models;
//using EdugameCloud.Lti.API;

//namespace EdugameCloud.Lti.Controllers
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Diagnostics;
//    using System.Diagnostics.CodeAnalysis;
//    using System.Linq;
//    using System.Net;
//    using System.Reflection;
//    using System.Runtime.Serialization;
//    using System.Web;
//    using System.Web.Mvc;
//    using System.Xml.Linq;
//    using System.Xml.XPath;
//    using DotNetOpenAuth.AspNet;
//    using EdugameCloud.Core.Domain.Entities;
//    using EdugameCloud.Lti.API.AdobeConnect;
//    using EdugameCloud.Lti.API.BlackBoard;
//    using EdugameCloud.Lti.API.Canvas;
//    using EdugameCloud.Lti.API.Desire2Learn;
//    using EdugameCloud.Lti.Constants;
//    using EdugameCloud.Lti.Core;
//    using EdugameCloud.Lti.Core.Business.Models;
//    using EdugameCloud.Lti.Core.Constants;
//    using EdugameCloud.Lti.Core.DTO;
//    using EdugameCloud.Lti.Core.OAuth;
//    using EdugameCloud.Lti.Domain.Entities;
//    using EdugameCloud.Lti.DTO;
//    using EdugameCloud.Lti.Extensions;
//    using EdugameCloud.Lti.Models;
//    using EdugameCloud.Lti.OAuth;
//    using EdugameCloud.Lti.OAuth.Canvas;
//    using EdugameCloud.Lti.OAuth.Desire2Learn;
//    using EdugameCloud.Lti.Utils;
//    using Esynctraining.AC.Provider;
//    using Esynctraining.AC.Provider.DataObjects.Results;
//    using Esynctraining.AC.Provider.Entities;
//    using Esynctraining.Core.Extensions;
//    using Esynctraining.Core.Providers;
//    using Esynctraining.Core.Utils;
//    using Microsoft.Web.WebPages.OAuth;
//    using Newtonsoft.Json;

//    [DataContract]
//    public class MeetingReuseDTO
//    {
//        [DataMember]
//        public string sco_id { get; set; }

//        [DataMember]
//        public string id { get; set; }

//    }

//    public partial class LtiController : Controller
//    {
//        [HttpPost]
//        public virtual JsonResult ReuseExistedAdobeConnecMeeting(string lmsProviderName, MeetingReuseDTO dto)
//        {
//            LmsCompany credentials = null;
//            try
//            {
//                var session = this.GetSession(lmsProviderName);
//                credentials = session.LmsCompany;
//                var param = session.LtiSession.With(x => x.LtiParam);
//                var provider = this.GetAdobeConnectProvider(credentials);
//                //OperationResult ret = this.meetingSetup.SaveMeeting(
//                //    credentials,
//                //    this.GetAdobeConnectProvider(credentials),
//                //    param,
//                //    meeting);


//                //param.course_id
//                //meeting.sco_id

//                // TODO:validate sco_id > 0

//                ScoInfoResult meetingSco = provider.GetScoInfo(dto.sco_id);
//                if (!meetingSco.Success)
//                {
//                    logger.ErrorFormat("[ReuseExistedAdobeConnecMeeting] Meeting not found in Adobe Connect. {0}.", meetingSco.Status.GetErrorInfo());
//                    throw new WarningMessageException("Meeting not found in Adobe Connect");
//                }

//                var meeting = new LmsCourseMeeting
//                {
//                    LmsCompany = credentials,
//                    CourseId = param.course_id,
//                    LmsMeetingType = (int)LmsMeetingType.Meeting,
//                    ScoId = dto.sco_id,
//                };

//                ////////////////////

//                var lmsUsers = new List<LmsUserDTO>();

//                bool retrieveLmsUsers = true;
//                if (retrieveLmsUsers)
//                {
//                    string error;
//                    lmsUsers = this.UsersSetup.GetLMSUsers(credentials,
//                            meeting,
//                            param.lms_user_id,
//                            meeting.CourseId,
//                            out error,
//                            param);
//                    if (error != null)
//                    {
//                        return OperationResult.Error("Unable retrieve information about LMS users.");
//                    }
//                }



//                CreateAnnouncement(
//                        (LmsMeetingType)meeting.LmsMeetingType,
//                        credentials,
//                        param,
//                        meetingDTO);


//                this.UsersSetup.SetDefaultUsers(
//                    credentials,
//                    meeting,
//                    provider,
//                    param.lms_user_id,
//                    meeting.CourseId,
//                    meeting.ScoId,
//                    lmsUsers,
//                    param);



//                if (meeting.LmsMeetingType == (int)LmsMeetingType.OfficeHours)
//                {
//                    officeHours = officeHours ?? new OfficeHours { LmsUser = lmsUser };
//                    officeHours.Hours = meetingDTO.office_hours;
//                    officeHours.ScoId = meeting.ScoId = result.ScoInfo.ScoId;

//                    this.OfficeHoursModel.RegisterSave(officeHours);

//                    meeting.OfficeHours = officeHours;
//                    meeting.ScoId = null;
//                    if (!isNewMeeting && meeting.Id == 0) // we attach existed office hours meeting for another course
//                    {
//                        CreateAnnouncement(
//                            (LmsMeetingType)meeting.LmsMeetingType,
//                            lmsCompany,
//                            param,
//                            meetingDTO);
//                    }
//                }
//                else if (meeting.LmsMeetingType == (int)LmsMeetingType.StudyGroup)
//                {
//                    meeting.Owner = lmsUser;
//                }

//                this.LmsCourseMeetingModel.RegisterSave(meeting);
//                this.LmsCourseMeetingModel.Flush();

//                SpecialPermissionId specialPermissionId = string.IsNullOrEmpty(meetingDTO.access_level)
//                                                              ? (meetingDTO.allow_guests
//                                                                     ? SpecialPermissionId.remove
//                                                                     : SpecialPermissionId.denied)
//                                                              : "denied".Equals(meetingDTO.access_level, StringComparison.OrdinalIgnoreCase)
//                                                                    ? SpecialPermissionId.denied
//                                                                    : ("view_hidden".Equals(meetingDTO.access_level, StringComparison.OrdinalIgnoreCase)
//                                                                           ? SpecialPermissionId.view_hidden
//                                                                           : SpecialPermissionId.remove);

//                provider.UpdatePublicAccessPermissions(result.ScoInfo.ScoId, specialPermissionId);
//                List<PermissionInfo> permission =
//                    provider.GetScoPublicAccessPermissions(result.ScoInfo.ScoId)
//                        .Values.Return(x => x.ToList(), new List<PermissionInfo>());

//                MeetingDTO updatedMeeting = this.GetMeetingDTOByScoInfo(
//                    lmsCompany,
//                    provider,
//                    param,
//                    result.ScoInfo,
//                    permission,
//                    meeting);

//                if (retrieveLmsUsers)
//                {
//                    string error;
//                    var users = this.UsersSetup.GetUsers(lmsCompany,
//                        provider,
//                        param,
//                        updatedMeeting.id,
//                        out error,
//                        lmsUsers);
//                    if (error != null)
//                    {
//                        return OperationResult.Error("Unable retrieve information about users.");
//                    }

//                    return OperationResult.Success(
//                        new MeetingAndLmsUsersDTO()
//                        {
//                            meeting = updatedMeeting,
//                            lmsUsers = users
//                        });
//                }

//                return OperationResult.Success(updatedMeeting);
//            }
//            catch (Exception ex)
//            {
//                string errorMessage = GetOutputErrorMessage("ReuseExistedAdobeConnecMeeting", credentials, ex);
//                return Json(OperationResult.Error(errorMessage));
//            }
//        }

//    }

//}