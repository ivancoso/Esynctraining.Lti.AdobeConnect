using System;
using System.Threading.Tasks;
using Esynctraining.Core.Json;
using Esynctraining.Lti.Lms.Common.Dto;
using Esynctraining.Lti.Zoom.Domain;
using Microsoft.EntityFrameworkCore;

namespace Esynctraining.Lti.Zoom.Common.Services
{
    public class UserSessionService
    {
        private readonly ZoomDbContext _dbContext;
        private readonly IJsonSerializer _jsonSerializer;


        public UserSessionService(ZoomDbContext dbContext,
            IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
            _dbContext = dbContext;
        }


        public async Task<LmsUserSession> SaveSession(Guid licenseKey, string courseId, LtiParamDTO param, string email,
            string lmsUserId)
        {
            var session = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                x.LicenseKey == licenseKey && x.CourseId == param.course_id.ToString() && x.LmsUserId == lmsUserId);
            if (session == null)
            {
                session = new LmsUserSession
                {
                    LicenseKey = licenseKey,
                    CourseId = param.course_id.ToString(),
                    LmsUserId = param.lms_user_id
                };

                _dbContext.Add(session);
            }

            session.Email = param.lis_person_contact_email_primary;

            //canvas only, need to resolve
            if (string.IsNullOrEmpty(session.Token))
            {
                var sessionWithToken = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                    x.LicenseKey == licenseKey && x.LmsUserId == lmsUserId && x.Token != null);
                if (sessionWithToken != null)
                    session.Token = sessionWithToken.Token;
            }

            session.SessionData = _jsonSerializer.JsonSerialize(param);
            await _dbContext.SaveChangesAsync();

            return session;
        }

        public async Task<LmsUserSession> GetSession(Guid licenseKey, string courseId, string lmsUserId)
        {
            var session = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x =>
                x.LicenseKey == licenseKey && x.CourseId == courseId && x.LmsUserId == lmsUserId);
            return session;
        }

        public async Task<LmsUserSession> GetSession(Guid id)
        {
            var session = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x => x.Id == id);
            return session;
        }

        public async Task<LmsUserSession> UpdateSessionRefreshToken(LmsUserSession session, string token, string refreshToken)
        {
            var lmsSession = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x => x.Id == session.Id);
            lmsSession.Token = token;
            lmsSession.RefreshToken = refreshToken;
            await _dbContext.SaveChangesAsync();
            return lmsSession;
        }

        public async Task<LmsUserSession> UpdateSessionAccessToken(LmsUserSession session, string token)
        {
            var lmsSession = await _dbContext.LmsUserSessions.FirstOrDefaultAsync(x => x.Id == session.Id);
            lmsSession.Token = token;
            await _dbContext.SaveChangesAsync();
            return lmsSession;
        }


    }

}