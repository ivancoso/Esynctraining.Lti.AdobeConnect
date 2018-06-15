using System;
using System.Linq;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Api.Services
{
    public class UserSessionService
    {
        private ZoomDbContext _dbContext;
        private IJsonSerializer _jsonSerializer;

        public UserSessionService(ZoomDbContext dbContext,
            IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
            _dbContext = dbContext;
        }

        public async Task<LmsUserSession> SaveSession(int licenseId, string courseId, LtiParamDTO param, string email,
            string lmsUserId)
        {
            var session = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                x.LicenseId == licenseId && x.CourseId == param.course_id.ToString() && x.LmsUserId == lmsUserId);
            if (session == null)
            {
                session = new LmsUserSession
                {
                    LicenseId = licenseId,
                    CourseId = param.course_id.ToString(),
                    Email = param.lis_person_contact_email_primary,
                    LmsUserId = param.lms_user_id
                };

                _dbContext.Add(session);
            }

            session.Email = param.lis_person_contact_email_primary;
            if (string.IsNullOrEmpty(session.Token))
            {
                var sessionWithToken = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                    x.LicenseId == licenseId && x.LmsUserId == lmsUserId && x.Token != null);
                if (sessionWithToken != null)
                    session.Token = sessionWithToken.Token;
            }


            session.SessionData = _jsonSerializer.JsonSerialize(param);
            await _dbContext.SaveChangesAsync();

            return session;
        }

        public async Task<LmsUserSession> GetSession(int licenseId, string courseId, string lmsUserId)
        {
            var session = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                x.LicenseId == licenseId && x.CourseId == courseId && x.LmsUserId == lmsUserId);
            return session;
        }

        public async Task<LmsUserSession> GetSession(Guid id)
        {
            var session = _dbContext.LmsUserSessions.FirstOrDefault(x =>
                x.Id == id);
            return session;
        }

        public async Task<LmsUserSession> UpdateSessionToken(LmsUserSession session, string token)
        {
            var lmsSession = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x => x.Id == session.Id);
            lmsSession.Token = token;
            await _dbContext.SaveChangesAsync();
            return lmsSession;
        }
    }
}