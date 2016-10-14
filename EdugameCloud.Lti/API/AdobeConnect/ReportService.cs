using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using EdugameCloud.Core.Business.Models;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using EdugameCloud.Lti.Extensions;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.Core.Logging;
using Esynctraining.Core.Utils;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public sealed class ReportService : IReportService
    {
        private static readonly string AcDateFormat = "yyyy-MM-ddTHH:mm"; // AdobeConnectProviderConstants.DateFormat

        #region Properties
        
        private LmsCourseMeetingModel LmsCourseMeetingModel
        {
            get { return IoC.Resolve<LmsCourseMeetingModel>(); }
        }

        private OfficeHoursModel OfficeHoursModel
        {
            get { return IoC.Resolve<OfficeHoursModel>(); }
        }

        private LmsUserModel LmsUserModel
        {
            get { return IoC.Resolve<LmsUserModel>(); }
        }

        private LmsUserParametersModel LmsUserParametersModel
        {
            get { return IoC.Resolve<LmsUserParametersModel>(); }
        }

        private LmsCompanyModel LmsСompanyModel
        {
            get { return IoC.Resolve<LmsCompanyModel>(); }
        }

        private UsersSetup UsersSetup
        {
            get { return IoC.Resolve<UsersSetup>(); }
        }

        private LmsFactory LmsFactory
        {
            get { return IoC.Resolve<LmsFactory>(); }
        }

        private LmsProviderModel LmsProviderModel
        {
            get { return IoC.Resolve<LmsProviderModel>(); }
        }


        private LanguageModel LanguageModel
        {
            get { return IoC.Resolve<LanguageModel>(); }
        }

        private IAdobeConnectUserService AcUserService
        {
            get { return IoC.Resolve<IAdobeConnectUserService>(); }
        }

        private IAdobeConnectAccountService AcAccountService
        {
            get { return IoC.Resolve<IAdobeConnectAccountService>(); }
        }

        private IAudioProfilesService AudioProfileService
        {
            get { return IoC.Resolve<IAudioProfilesService>(); }
        }

        private ILogger Logger
        {
            get { return IoC.Resolve<ILogger>(); }
        }

        #endregion

        #region Public Methods and Operators
        
        public List<ACSessionDTO> GetSessionsReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (provider == null)
                throw new ArgumentNullException(nameof(meeting));

            return GetSessionsWithParticipants(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }
        
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<ACSessionParticipantDTO> GetAttendanceReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (provider == null)
                throw new ArgumentNullException(nameof(meeting));

            return GetAttendanceReport(meeting.GetMeetingScoId(), provider, startIndex, limit);
        }

        public IEnumerable<RecordingTransactionDTO> GetRecordingsReport(Esynctraining.AdobeConnect.IAdobeConnectProxy provider, LmsCourseMeeting meeting, int startIndex = 0, int limit = 0)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (provider == null)
                throw new ArgumentNullException(nameof(meeting));

            try
            {
                var recordingsSco = provider.GetRecordingsList(meeting.GetMeetingScoId()).Values.Select(x => x.ScoId);
                if (!recordingsSco.Any())
                    return Enumerable.Empty<RecordingTransactionDTO>();

                var transactions = provider.ReportRecordingTransactions(recordingsSco, startIndex, limit).Values.ToList();
                return transactions.Select(
                        us =>
                        new RecordingTransactionDTO
                        {
                            RecordingScoId = us.ScoId,
                            RecordingName = us.Name,
                            Login = us.Login,
                            UserName = us.UserName,
                            DateClosed = us.DateClosed,
                            DateCreated = us.DateCreated,
                        }).OrderByDescending(x => x.DateCreated).ToList();

            }
            catch (Exception ex)
            {
                Logger.Error("GetRecordingsReport.Exception", ex);
                return Enumerable.Empty<RecordingTransactionDTO>();
            }
        }
        
        #endregion

        #region Methods
        
        private List<ACSessionParticipantDTO> GetAttendanceReport(string meetingId, Esynctraining.AdobeConnect.IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                var meetingAttendees = acp.ReportMeetingAttendance(meetingId, startIndex, limit).Values.ToList();
                return meetingAttendees.Select(
                        us =>
                        new ACSessionParticipantDTO
                        {
                            scoId = us.ScoId,
                            scoName = us.ScoName,
                            participantName = us.ParticipantName,
                            assetId = us.AssetId,
                            sessionName = us.SessionName,
                            principalId = us.PrincipalId,
                            firstName = us.SessionName,
                            login = us.Login,
                            dateTimeEntered = us.DateCreated,
                            dateTimeLeft = us.DateEnd.FixACValue(),

                            // TODO: REVIEW!!! HACK
                            durationInHours = (float)us.Duration.TotalHours,
                            transcriptId = int.Parse(us.TranscriptId)
                        }).OrderByDescending(x => x.dateTimeEntered).ToList();
                
            }
            catch (Exception ex)
            {
                Logger.Error("GetAttendanceReport.Exception", ex);
            }

            return new List<ACSessionParticipantDTO>();
        }
        
        private static List<ACSessionDTO> GetSessionsWithParticipantsBySessionTime(string meetingId, List<MeetingAttendee> meetingAttendees,
            Esynctraining.AdobeConnect.IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            List<MeetingSession> sessions = acp.ReportMeetingSessions(meetingId, startIndex, limit).Values.ToList();
            var result = sessions.Select(sco => new ACSessionDTO
            {
                scoId = int.Parse(sco.ScoId),
                assetId = int.Parse(sco.AssetId),
                dateStarted = sco.DateCreated.FixACValue(),
                dateEnded = sco.DateEnd.FixACValue(),
                sessionNumber = int.Parse(sco.Version),
                sessionName = sco.Version,
                participants = new List<ACSessionParticipantDTO>()
            }).ToList();

            var unprocessedAttendees = new List<MeetingAttendee>();
            foreach (var attendee in meetingAttendees)
            {
                var session = result.FirstOrDefault(x => x.dateStarted <= attendee.DateCreated && (!x.dateEnded.HasValue || attendee.DateEnd <= x.dateEnded));
                if (session != null)
                {
                    session.participants.Add(
                        new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float) attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        });
                    if (String.IsNullOrEmpty(session.meetingName))
                    {
                        session.meetingName = attendee.ScoName;
                    }
                }
                else
                {
                    unprocessedAttendees.Add(attendee);
                }
            }
            //unlikely possible case, need to check
            if (unprocessedAttendees.Count > 0)
            {
                var maxDate = result.Max(x => x.dateStarted);
                var ua = unprocessedAttendees.Where(x => x.DateCreated >= maxDate);
                if (ua.Any())
                {
                    var currentSessionNumber = result.Max(x => x.sessionNumber);
                    result.Add(new ACSessionDTO
                    {
                        sessionNumber = currentSessionNumber + 1,
                        sessionName = (currentSessionNumber + 1).ToString(CultureInfo.CurrentCulture),
                        dateStarted = ua.Min(x => x.DateCreated),
                        participants = ua.Select(attendee => new ACSessionParticipantDTO
                        {
                            firstName = attendee.SessionName,
                            login = attendee.Login,
                            dateTimeEntered = attendee.DateCreated,
                            dateTimeLeft = attendee.DateEnd.FixACValue(),
                            durationInHours =
                                (float)attendee.Duration.TotalHours,
                            transcriptId = int.Parse(attendee.TranscriptId)
                        }).ToList(),
                        meetingName = ua.First().ScoName
                    });
                }
            }

            GroupSessionParticipants(result);
            return result.OrderBy(s => s.sessionNumber).ToList();
        }

        private List<ACSessionDTO> GetSessionsWithParticipants(string meetingId, Esynctraining.AdobeConnect.IAdobeConnectProxy acp, int startIndex = 0, int limit = 0)
        {
            try
            {
                List<MeetingAttendee> meetingAttendees = acp.ReportMeetingAttendance(meetingId).Values.ToList();
                if (meetingAttendees.All(x => string.IsNullOrEmpty(x.AssetId)))
                {
                    //todo: we should not rely on AssetId parameter and probably use following method in all cases
                    return GetSessionsWithParticipantsBySessionTime(meetingId, meetingAttendees, acp, startIndex, limit);
                }

                //left previous version to avoid any possible errors
                var userSessions = meetingAttendees.Where(x => !string.IsNullOrEmpty(x.AssetId))
                    .GroupBy(v => v.AssetId, v => v)
                    .ToDictionary(g => int.Parse(g.Key), g => g.ToList());

                var sessions = acp.ReportMeetingSessions(meetingId, startIndex, limit).Values.ToList();

                var sessionList =
                    (from asset in userSessions.Keys.Except(sessions.ConvertAll(s => int.Parse(s.AssetId)))
                        let index =
                            sessions.Any(s => !string.IsNullOrEmpty(s.Version))
                                ? sessions.Max(s => int.Parse(s.Version)) + 1
                                : 0
                        select
                            new ACSessionDTO
                            {
                                assetId = asset,
                                sessionNumber = index,
                                sessionName = index.ToString(CultureInfo.CurrentCulture)
                            }).ToList();
                sessions.AddRange(
                    sessionList.Select(
                        s => new MeetingSession { AssetId = s.assetId.ToString(CultureInfo.CurrentCulture) }));

                foreach (var sco in sessions)
                {
                    var session = sessionList.FirstOrDefault(s => s.assetId == int.Parse(sco.AssetId));
                    if (null == session)
                    {
                        session = new ACSessionDTO
                        {
                            scoId = int.Parse(sco.ScoId),
                            assetId = int.Parse(sco.AssetId),
                            dateStarted = sco.DateCreated.FixACValue(),
                            dateEnded = sco.DateEnd.FixACValue(),
                            sessionNumber = int.Parse(sco.Version),
                            sessionName = sco.Version,
                            participants = new List<ACSessionParticipantDTO>(),
                        };

                        sessionList.Add(session);
                    }

                    foreach (var us in userSessions[session.assetId])
                    {
                        var participant = new ACSessionParticipantDTO
                        {
                            firstName = us.SessionName,
                            login = us.Login,
                            dateTimeEntered = us.DateCreated,
                            dateTimeLeft = us.DateEnd.FixACValue(),
                            durationInHours =
                                (float) us.Duration.TotalHours,
                            transcriptId = int.Parse(us.TranscriptId),
                        };

                        session.meetingName = us.ScoName;
                        session.participants.Add(participant);
                    }

                    if (!session.dateStarted.HasValue)
                    {
                        session.dateStarted = session.participants.Min(p => p.dateTimeEntered);
                    }
                }

                GroupSessionParticipants(sessionList);

                return sessionList.OrderBy(s => s.sessionNumber).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error("GetSessionsWithParticipants.Exception", ex);
            }

            return new List<ACSessionDTO>();
        }

        private static void GroupSessionParticipants(IEnumerable<ACSessionDTO> sessionList)
        {
            foreach (var session in sessionList)
            {
                var singleAttendance = session.participants.GroupBy(p => p.loginOrFullName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var attendance in singleAttendance.Where(a => !string.IsNullOrWhiteSpace(a.Key) && a.Value.Count > 1))
                {
                    attendance.Value.Skip(1).ToList().ForEach(p => session.participants.Remove(p));
                    var attendee = attendance.Value.First();
                    attendee.dateTimeEntered = attendance.Value.Min(p => p.dateTimeEntered);
                    attendee.dateTimeLeft = attendance.Value.Max(p => p.dateTimeLeft);
                    attendee.durationInHours = attendance.Value.Sum(p => p.durationInHours);
                }
            }
        }
        
        #endregion

    }

}
