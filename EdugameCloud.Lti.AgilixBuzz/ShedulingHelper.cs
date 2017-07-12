//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using EdugameCloud.Lti.API.AdobeConnect;
//using EdugameCloud.Lti.API.AgilixBuzz;
//using EdugameCloud.Lti.Core.Business.Models;
//using EdugameCloud.Lti.Domain.Entities;
//using EdugameCloud.Lti.DTO;
//using Esynctraining.AdobeConnect;
//using Esynctraining.AdobeConnect.Api.Meeting.Dto;
//using Esynctraining.Core.Caching;
//using Esynctraining.Core.Extensions;
//using Esynctraining.Core.Utils;

//namespace EdugameCloud.Lti.AgilixBuz
//{
//    public sealed class ShedulingHelper : IAgilixBuzzScheduling
//    {
//        private readonly DlapAPI _dlapApi;
//        private readonly IMeetingSetup _meetingSetup;
//        private readonly IUsersSetup _usersSetup;
//        private readonly LmsCompanyModel _lmsCompanyModel;
//        private API.AdobeConnect.IAdobeConnectAccountService _acAccountService;


//        public ShedulingHelper(DlapAPI dlapApi, IMeetingSetup meetingSetup, IUsersSetup usersSetup, LmsCompanyModel lmsCompanyModel, API.AdobeConnect.IAdobeConnectAccountService acAccountService)
//        {
//            _dlapApi = dlapApi;
//            _meetingSetup = meetingSetup;
//            _usersSetup = usersSetup;
//            _lmsCompanyModel = lmsCompanyModel;
//            _acAccountService = acAccountService;
//        }


//        /// <summary>
//        /// The get group processing signal type.
//        /// </summary>
//        /// <param name="group">
//        /// The group.
//        /// </param>
//        /// <returns>
//        /// The <see cref="ProcessingSignalType"/>.
//        /// </returns>
//        private static ProcessingSignalType GetGroupProcessingSignalType(List<Signal> @group)
//        {
//            if (group.Any(x => x.Type == DlapAPI.SignalTypes.CourseDeleted))
//            {
//                return group.Any(x => x.Type == DlapAPI.SignalTypes.CourseCreated)
//                           ? ProcessingSignalType.Skip
//                           : ProcessingSignalType.CourseToDelete;
//            }

//            return ProcessingSignalType.CourseToProcess;
//        }

//        /// <summary>
//        /// The parse.
//        /// </summary>
//        /// <param name="orderedList">
//        /// The ordered list.
//        /// </param>
//        /// <returns>
//        /// The <see cref="List{ParsedSignalGroup}"/>.
//        /// </returns>
//        private static List<ParsedSignalGroup> Parse(List<Signal> orderedList)
//        {
//            var result = new List<ParsedSignalGroup>();
//            List<Signal> enrollments = orderedList.Where(x => x.Type == DlapAPI.SignalTypes.EnrollmentChanged).ToList();
//            List<Signal> courses = orderedList.Except(enrollments).ToList();
//            Dictionary<int, List<Signal>> coursesGrouped = courses.GroupBy(x => x.EntityId)
//                .ToDictionary(x => x.Key, x => x.ToList());
//            foreach (int key in coursesGrouped.Keys)
//            {
//                List<Signal> group = coursesGrouped[key];
//                result.Add(
//                    new ParsedSignalGroup
//                    {
//                        ProcessingSignalType = GetGroupProcessingSignalType(group),
//                        RepresentativeSignal = group.FirstOrDefault(x => x.EntityId != 0),
//                        SignalsAssociated = group
//                    });
//            }

//            if (enrollments.Any())
//            {
//                result.Add(
//                    new ParsedSignalGroup
//                    {
//                        ProcessingSignalType = ProcessingSignalType.SoleEnrollment,
//                        SignalsAssociated = enrollments
//                    });
//            }

//            return result;
//        }

//        /// <summary>
//        /// The remove course enrollments from sole enrollments.
//        /// </summary>
//        /// <param name="soleEnrollments">
//        /// The sole enrollments.
//        /// </param>
//        /// <param name="courseUsers">
//        /// The course users.
//        /// </param>
//        private static void RemoveCourseEnrollmentsFromSoleEnrollments(
//            List<Signal> soleEnrollments,
//            List<LmsUserDTO> courseUsers)
//        {
//            if (courseUsers != null && courseUsers.Any())
//            {
//                List<int> courseIds = courseUsers.Select(x => int.Parse(x.Id)).ToList();
//                List<Signal> toRemove = soleEnrollments.Where(x => courseIds.Contains(x.EntityId)).ToList();
//                foreach (Signal signal in toRemove)
//                {
//                    soleEnrollments.Remove(signal);
//                }
//            }
//        }

//        public string CheckForAgilixBuzzSignals(IEnumerable<LmsCompany> agilixBuzzCompanies, DateTime lastScheduledRunDate, int meetingId)
//        {
//            var errors = new List<string>();
//            DlapAPI api = _dlapApi;

//            var cache = IoC.Resolve<ICache>();

//            foreach (LmsCompany agilixBuzzCompany in agilixBuzzCompanies)
//            {
//                string error;
//                Session session = api.BeginBatch(out error, agilixBuzzCompany);

//                if (error != null)
//                {
//                    AddToErrors(errors, error, agilixBuzzCompany);
//                }
//                else
//                {
//                    if (agilixBuzzCompany.LastSignalId == 0)
//                    {
//                        long? signalId = api.GetLastSignalId(agilixBuzzCompany, out error, session);
//                        if (error != null)
//                        {
//                            AddToErrors(errors, error, agilixBuzzCompany);
//                        }
//                        else if (signalId.HasValue)
//                        {
//                            this.UpdateLastSignalId(agilixBuzzCompany, signalId.Value);
//                        }

//                        continue;
//                    }

//                    IAdobeConnectProxy adobeConnectProvider = _acAccountService.GetProvider(agilixBuzzCompany);
//                    IEnumerable<TemplateDto> templates = _acAccountService.GetSharedMeetingTemplates(adobeConnectProvider, cache);
//                    if (!templates.Any())
//                    {
//                        error = "No templates found for " + agilixBuzzCompany.LmsDomain;
//                    }

//                    if (error != null)
//                    {
//                        AddToErrors(errors, error, agilixBuzzCompany);
//                    }
//                    else
//                    {
//                        List<Signal> signals = api.GetSignalsList(agilixBuzzCompany, out error, session);
//                        if (error != null)
//                        {
//                            AddToErrors(errors, error, agilixBuzzCompany);
//                        }
//                        else
//                        {
//                            List<ParsedSignalGroup> parsedSignals = Parse(
//                                signals.OrderBy(x => x.SignalId).ToList());
//                            List<Signal> soleEnrollments =
//                                parsedSignals.FirstOrDefault(
//                                    x => x.ProcessingSignalType == ProcessingSignalType.SoleEnrollment)
//                                    .Return(x => x, new ParsedSignalGroup { SignalsAssociated = new List<Signal>() })
//                                    .SignalsAssociated;
//                            var userNamesAndEmailsDeleted = new Dictionary<int, List<string>>();
//                            foreach (ParsedSignalGroup signalGroup in parsedSignals)
//                            {
//                                switch (signalGroup.ProcessingSignalType)
//                                {
//                                    case ProcessingSignalType.CourseToProcess:
//                                        List<LmsUserDTO> courseUsers =
//                                            ProcessCourseCreated(
//                                                signalGroup.RepresentativeSignal,
//                                                api,
//                                                agilixBuzzCompany,
//                                                session,
//                                                errors,
//                                                adobeConnectProvider,
//                                                templates);
//                                        RemoveCourseEnrollmentsFromSoleEnrollments(soleEnrollments, courseUsers);
//                                        break;

//                                    case ProcessingSignalType.CourseToDelete:
//                                        List<string> courseDeletedUsers =
//                                            ProcessCourseDeleted(
//                                                signalGroup.RepresentativeSignal,
//                                                agilixBuzzCompany,
//                                                errors,
//                                                adobeConnectProvider,
//                                                meetingId);
//                                        int key = signalGroup.RepresentativeSignal.EntityId;
//                                        if (!userNamesAndEmailsDeleted.ContainsKey(key))
//                                        {
//                                            userNamesAndEmailsDeleted.Add(key, new List<string>());
//                                        }

//                                        userNamesAndEmailsDeleted[key].AddRange(courseDeletedUsers);
//                                        break;
//                                }
//                            }

//                            ProcessSoleEnrollments(
//                                agilixBuzzCompany,
//                                soleEnrollments,
//                                errors,
//                                api,
//                                session,
//                                adobeConnectProvider,
//                                meetingId);

//                            Signal lastSignal = signals.LastOrDefault();
//                            if (lastSignal != null)
//                            {
//                                this.UpdateLastSignalId(agilixBuzzCompany, lastSignal.SignalId);
//                            }
//                        }
//                    }
//                }
//            }

//            return errors.ToPlainString();
//        }

//        private List<LmsUserDTO> ProcessCourseCreated(
//            Signal signal,
//            DlapAPI api,
//            LmsCompany agilixBuzzCompany,
//            Session session,
//            List<string> errors,
//            IAdobeConnectProxy adobeConnectProvider,
//            IEnumerable<TemplateDto> templates)
//        {
//            var result = new List<LmsUserDTO>();
//            string error;
//            var courseSignal = (CourseSignal)signal;
//            Course course = api.GetCourse(agilixBuzzCompany, courseSignal.EntityId, out error, session);
//            if (error != null)
//            {
//                AddToErrors(errors, error, agilixBuzzCompany);
//            }
//            else
//            {
//                List<LmsUserDTO> courseEnrolledUsers = api.GetUsersForCourse(
//                    agilixBuzzCompany,
//                    courseSignal.EntityId,
//                    out error,
//                    session);
//                if (error == null && courseEnrolledUsers != null && courseEnrolledUsers.Any())
//                {
//                    result.AddRange(courseEnrolledUsers);
//                }

//                throw new NotImplementedException("TODO: it seems this API doesnt work and nobody calls it");

//                var startDate = DateTime.Parse(course.StartDate);
//                var trace = new StringBuilder();

//                var param = new LtiParamDTO
//                {
//                    context_id = course.CourseId.ToString(CultureInfo.InvariantCulture),
//                    tool_consumer_info_product_family_code = "Buzz"
//                };
//                var useLmsUserEmailForSearch = !string.IsNullOrEmpty(param.lis_person_contact_email_primary);
//                var fb = new MeetingFolderBuilder(agilixBuzzCompany, adobeConnectProvider, useLmsUserEmailForSearch);

//                _meetingSetup.SaveMeeting(
//                    agilixBuzzCompany,
//                    adobeConnectProvider,
//                    param,
//                    new MeetingDTOInput
//                    {
//                        StartDate = startDate.ToString("MM-dd-yyyy"),
//                        StartTime = startDate.ToString("hh:mm tt"),
//                        Duration = "01:00",

//                        // TODO: review it!!
//                        //id = courseSignal.ItemId,

//                        Name = course.Title,
//                        Template = templates.First().With(x => x.Id)
//                    },
//                    trace,
//                    fb);
//            }

//            return result;
//        }

//        /// <summary>
//        /// The process course created.
//        /// </summary>
//        /// <param name="signal">
//        /// The signal.
//        /// </param>
//        /// <param name="agilixBuzzCompany">
//        /// The agilixBuzz Company.
//        /// </param>
//        /// <param name="errors">
//        /// The errors.
//        /// </param>
//        /// <param name="adobeConnectProvider">
//        /// The adobe connect provider.
//        /// </param>
//        /// <param name="scoId">
//        /// The sco Id.
//        /// </param>
//        /// <returns>
//        /// The <see cref="List{String}"/>.
//        /// </returns>
//        private List<string> ProcessCourseDeleted(
//            Signal signal,
//            LmsCompany agilixBuzzCompany,
//            List<string> errors,
//            IAdobeConnectProxy adobeConnectProvider,
//            int meetingId)
//        {
//            string error;
//            List<string> result = this._meetingSetup.DeleteMeeting(
//                agilixBuzzCompany,
//                adobeConnectProvider,
//                new LtiParamDTO { context_id = signal.EntityId.ToString(CultureInfo.InvariantCulture) },
//                meetingId,
//                out error);
//            if (error != null)
//            {
//                AddToErrors(errors, error, agilixBuzzCompany);
//            }

//            return result;
//        }

//        // ReSharper disable once UnusedParameter.Local
//        private void ProcessSoleEnrollments(
//            LmsCompany agilixBuzzCompany,
//            IEnumerable<Signal> soleEnrollments,
//            List<string> errors,
//            DlapAPI api,
//            Session session,
//            IAdobeConnectProxy provider,
//            int meetingId)
//        {
//            Dictionary<int, IGrouping<int, Signal>> grouped =
//                soleEnrollments.OrderBy(x => x.SignalId)
//                    .GroupBy(x => x.EntityId, x => x)
//                    .ToDictionary(x => x.Key, x => x);
//            foreach (int entityId in grouped.Keys)
//            {
//                EnrollmentSignal latestSignal = grouped[entityId].LastOrDefault().With(x => (EnrollmentSignal)x);
//                if (latestSignal != null)
//                {
//                    if (latestSignal.NewStatus != 0)
//                    {
//                        string error;
//                        Enrollment enrollment = api.GetEnrollment(
//                            agilixBuzzCompany,
//                            latestSignal.EntityId,
//                            out error,
//                            session);
//                        if (error != null)
//                        {
//                            AddToErrors(errors, error, agilixBuzzCompany);
//                        }
//                        else if (enrollment != null && api.IsEnrollmentActive(enrollment.Status))
//                        {
//                            var lmsUser = new LmsUserDTO
//                            {
//                                Login = enrollment.UserName,
//                                PrimaryEmail = enrollment.Email,
//                                LmsRole = enrollment.Role,
//                                Id = enrollment.UserId,
//                                IsEditable = true,
//                            };
//                            //this._usersSetup.SetLMSUserDefaultACPermissions(provider, agilixBuzzCompany, null, lmsUser, null);
//                            this._usersSetup.UpdateUser(
//                                agilixBuzzCompany,
//                                provider,
//                                new LtiParamDTO
//                                {
//                                    context_id = enrollment.CourseId.ToString(CultureInfo.InvariantCulture)
//                                },
//                                lmsUser,
//                                meetingId,
//                                out error,
//                                true);
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// The update last signal id.
//        /// </summary>
//        /// <param name="agilixBuzzCompany">
//        /// The agilixBuzz Company.
//        /// </param>
//        /// <param name="signalId">
//        /// The signal id.
//        /// </param>
//        private void UpdateLastSignalId(LmsCompany agilixBuzzCompany, long signalId)
//        {
//            agilixBuzzCompany.LastSignalId = signalId;
//            this._lmsCompanyModel.RegisterSave(agilixBuzzCompany);
//        }

//        /// <summary>
//        /// The add to errors.
//        /// </summary>
//        /// <param name="errors">
//        /// The errors.
//        /// </param>
//        /// <param name="error">
//        /// The error.
//        /// </param>
//        /// <param name="agilixBuzzCompany">
//        /// The agilixBuzz company.
//        /// </param>
//        private static void AddToErrors(List<string> errors, string error, LmsCompany agilixBuzzCompany)
//        {
//            errors.Add(string.Format("Error with company {0}:{1}", agilixBuzzCompany.With(x => x.LmsDomain), error));
//        }
//    }

//}
