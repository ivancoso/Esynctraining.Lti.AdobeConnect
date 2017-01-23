using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AdobeConnect.Api.MeetingReports.Dto;
using Esynctraining.Core.Logging;

namespace Esynctraining.AdobeConnect.Api.MeetingReports
{
    public class ReportsService : IReportsService
    {
        private readonly ILogger logger;


        public ReportsService(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            this.logger = logger;
        }


        public IEnumerable<ACSessionDto> GetSessionsReports(string meetingId, IAdobeConnectProxy ac, TimeZoneInfo timeZone, int startIndex = 0, int limit = 0)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingId));
            if (ac == null)
                throw new ArgumentNullException(nameof(ac));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            try
            {
                List<MeetingAttendee> meetingAttendees = ac.ReportMeetingAttendance(meetingId).Values.ToList();
                if (meetingAttendees.All(x => string.IsNullOrEmpty(x.AssetId)))
                {
                    //todo: we should not rely on AssetId parameter and probably use following method in all cases
                    return GetSessionsWithParticipantsBySessionTime(meetingId, meetingAttendees, ac, timeZone, startIndex, limit);
                }

                //left previous version to avoid any possible errors
                var userSessions = meetingAttendees.Where(x => !string.IsNullOrEmpty(x.AssetId))
                    .GroupBy(v => v.AssetId, v => v)
                    .ToDictionary(g => g.Key, g => g.ToList());

                var sessions = ac.ReportMeetingSessions(meetingId, startIndex: startIndex, limit: limit).Values.ToList();

                var sessionList =
                    (from asset in userSessions.Keys.Except(sessions.ConvertAll(s => s.AssetId))
                     let index =
                         sessions.Any(s => !string.IsNullOrEmpty(s.Version))
                             ? sessions.Max(s => int.Parse(s.Version)) + 1
                             : 0
                     select
                         new ACSessionDto
                         {
                             assetId = asset.ToString(),
                             sessionNumber = index,
                             //sessionName = index.ToString(CultureInfo.CurrentCulture)
                         }).ToList();
                sessions.AddRange(
                    sessionList.Select(
                        s => new MeetingSession { AssetId = s.assetId.ToString(CultureInfo.CurrentCulture) }));


                foreach (var sco in sessions)
                {
                    var session = sessionList.FirstOrDefault(s => s.assetId == sco.AssetId);
                    if (null == session)
                    {

                        session = new ACSessionDto(sco, timeZone);
                        sessionList.Add(session);
                    }

                    foreach (var us in userSessions[session.assetId])
                    {
                        var participant = new ACSessionParticipantDto(us, timeZone);

                        //session.meetingName = us.ScoName;
                        session.participants.Add(participant);
                    }

                    if (!session.dateStarted.HasValue)
                    {
                        session.dateStarted = FixACValue(session.participants.Min(p => p.dateTimeEntered), timeZone).Value;
                    }
                }

                GroupSessionParticipants(sessionList, timeZone);

                return sessionList.OrderBy(s => s.sessionNumber).ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetSessionsReports.Exception", ex);
            }

            return new List<ACSessionDto>();
        }


        public IEnumerable<ACSessionParticipantDto> GetAttendanceReports(string meetingId, IAdobeConnectProxy ac, TimeZoneInfo timeZone, int startIndex = 0, int limit = 0)
        {
            if (string.IsNullOrWhiteSpace(meetingId))
                throw new ArgumentException("Non-empty value expected", nameof(meetingId));
            if (ac == null)
                throw new ArgumentNullException(nameof(ac));
            if (timeZone == null)
                throw new ArgumentNullException(nameof(timeZone));

            try
            {
                var meetingAttendees = ac.ReportMeetingAttendance(meetingId, startIndex, limit).Values.ToList();
                return meetingAttendees
                    .Select(us => new ACSessionParticipantDto(us, timeZone))
                    .OrderBy(x => x.participantName)
                    .ThenBy(x => x.dateTimeEntered)
                    .ToList();
            }
            catch (Exception ex)
            {
                logger.Error("GetAttendanceReports.Exception", ex);
            }

            return Enumerable.Empty<ACSessionParticipantDto>();
        }

        private void GroupSessionParticipants(IEnumerable<ACSessionDto> sessionList, TimeZoneInfo timeZone)
        {
            foreach (var session in sessionList)
            {
                var singleAttendance = session.participants.GroupBy(p => p.loginOrFullName)
                    .ToDictionary(g => g.Key, g => g.ToList());
                foreach (var attendance in singleAttendance.Where(a => !string.IsNullOrWhiteSpace(a.Key) && a.Value.Count > 1))
                {
                    attendance.Value.Skip(1).ToList().ForEach(p => session.participants.Remove(p));
                    var attendee = attendance.Value.First();
                    attendee.dateTimeEntered = DateTime.SpecifyKind(attendance.Value.Min(p => p.dateTimeEntered), DateTimeKind.Utc);
                    attendee.dateTimeLeft = FixACValue(attendance.Value.Max(p => p.dateTimeLeft), timeZone);

                    attendee.durationInHours = attendance.Value.Sum(p => p.durationInHours);
                }
            }
        }

        private List<ACSessionDto> GetSessionsWithParticipantsBySessionTime(string meetingId, List<MeetingAttendee> meetingAttendees,
            IAdobeConnectProxy acp, TimeZoneInfo timeZone, int startIndex = 0, int limit = 0)
        {
            List<MeetingSession> sessions = acp.ReportMeetingSessions(meetingId, startIndex: startIndex, limit: limit).Values.ToList();
            var result = sessions.Select(sco => new ACSessionDto(sco, timeZone)).ToList();

            var unprocessedAttendees = new List<MeetingAttendee>();
            foreach (var attendee in meetingAttendees)
            {
                var session = result.FirstOrDefault(x => x.dateStarted <= attendee.DateCreated && (!x.dateEnded.HasValue || attendee.DateEnd <= x.dateEnded));
                if (session != null)
                {
                    session.participants.Add(new ACSessionParticipantDto(attendee, timeZone));
                    //if (string.IsNullOrEmpty(session.meetingName))
                    //{
                    //    session.meetingName = attendee.ScoName;
                    //}
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
                    result.Add(new ACSessionDto
                    {
                        sessionNumber = currentSessionNumber + 1,
                        //sessionName = (currentSessionNumber + 1).ToString(CultureInfo.CurrentCulture),
                        dateStarted = ua.Min(x => x.DateCreated),
                        participants = ua.Select(attendee => new ACSessionParticipantDto(attendee, timeZone)).ToList(),
                        //meetingName = ua.First().ScoName,
                    });
                }
            }

            GroupSessionParticipants(result, timeZone);
            return result.OrderBy(s => s.sessionNumber).ToList();
        }


        private DateTime? FixACValue(DateTime? dt, TimeZoneInfo timeZone)
        {
            if (dt.HasValue)
            {
                return FixACValue(dt.Value, timeZone);
            }
            return null;
        }

        private DateTime? FixACValue(DateTime dt, TimeZoneInfo timeZone)
        {
            var tmp = dt < dt1951 ? (DateTime?)null : new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond, DateTimeKind.Utc);

            if (tmp.HasValue)
            {
                return DateTime.SpecifyKind(TimeZoneInfo.ConvertTimeFromUtc(tmp.Value, timeZone), DateTimeKind.Utc);
            }
            return null;
        }

        private readonly DateTime dt1951 = new DateTime(1951, 1, 1);

    }

}