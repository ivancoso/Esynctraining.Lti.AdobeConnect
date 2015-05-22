using System;
using System.Collections.Generic;
using System.Linq;
using BbWsClient;
using Castle.Core.Logging;
using EdugameCloud.Lti.API;
using EdugameCloud.Lti.API.BlackBoard;
using EdugameCloud.Lti.Core.Business.Models;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.Core.Extensions;
using Esynctraining.Core.Providers;
using Newtonsoft.Json;

namespace EdugameCloud.Lti.BlackBoard
{
    public class BlackboardLmsUserService : LmsUserServiceBase
    {
        private static readonly Dictionary<string, object> locker = new Dictionary<string, object>();
        private readonly LmsCourseMeetingModel lmsCourseMeetingModel;
        private readonly dynamic settings;
        private readonly IBlackBoardApi soapApi;

        public BlackboardLmsUserService(ILogger logger, LmsCourseMeetingModel lmsCourseMeetingModel,
            IBlackBoardApi soapApi, 
            ApplicationSettingsProvider settings
            ) : base(logger)
        {
            this.soapApi = soapApi; 
            this.lmsCourseMeetingModel = lmsCourseMeetingModel;
            this.settings = settings;
        }

        public override LmsUserDTO GetUser(LmsCompany lmsCompany, LmsUser currentUser, LmsCourseMeeting meeting, string lmsUserId, int courseId, out string error, object extraData = null, bool forceUpdate = false)
        {
            Guid guid;
            return GetUsersOldStyle(lmsCompany, meeting, lmsUserId, courseId, out error, forceUpdate)
                .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.lti_id : u.id));
        }

        public override bool CanRetrieveUsersFromApiForCompany(LmsCompany lmsCompany)
        {
            return lmsCompany.AdminUser != null || (lmsCompany.EnableProxyToolMode.GetValueOrDefault() && lmsCompany.ProxyToolSharedPassword != null);
        }

        public override OperationResult<List<LmsUserDTO>> GetUsers(LmsCompany lmsCompany, LmsCourseMeeting meeting, 
            LmsUser lmsUser, int courseId, object extraData = null, bool forceUpdate = false)
        {
            string error;
            var users = GetUsersOldStyle(lmsCompany, meeting, lmsUser.UserId, courseId, out error, forceUpdate);
            return OperationResult<List<LmsUserDTO>>.Success(users);
        }

        public override List<LmsUserDTO> GetUsersOldStyle(LmsCompany lmsCompany, LmsCourseMeeting meeting, 
            string userId, int courseId, out string error, bool forceUpdate = false, object param = null)
        {
            TimeSpan timeout = TimeSpan.Parse((string)this.settings.UserCacheValidTimeout);
            string key = lmsCompany.LmsDomain + ".course." + courseId;
            error = null;
            List<LmsUserDTO> cachedUsers = CheckCachedUsers(meeting, forceUpdate, timeout);
            if (cachedUsers == null)
            {
                object lockMe = GetLocker(key);
                lock (lockMe)
                {
                    if (meeting != null)
                    {
                        lmsCourseMeetingModel.Refresh(ref meeting);
                    }

                    cachedUsers = CheckCachedUsers(meeting, forceUpdate, timeout);
                    if (cachedUsers == null)
                    {
                        WebserviceWrapper client = null;
                        List<LmsUserDTO> users = this.soapApi.GetUsersForCourse(
                            lmsCompany,
                            courseId,
                            out error,
                            ref client);

                        if ((users.Count == 0)
                            && error.Return(
                                x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0,
                                false))
                        {
                            logger.Warn("GetBlackBoardUsers.AccessDenied. " + error);

                            // NOTE: set to null to re-create session.
                            client = null;
                            users = this.soapApi.GetUsersForCourse(
                                lmsCompany,
                                courseId,
                                out error,
                                ref client);
                        }

                        // TODO: try to call logout
                        // client.logout();
                        if (string.IsNullOrWhiteSpace(error) && (meeting != null))
                        {
                            meeting.AddedToCache = DateTime.Now;
                            meeting.CachedUsers = JsonConvert.SerializeObject(users);
                            lmsCourseMeetingModel.RegisterSave(meeting, true);
                        }
                        else if ((users.Count == 0)
                                 && error.Return(
                                     x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0,
                                     false))
                        {
                            users = CheckCachedUsers(meeting, false, timeout) ?? new List<LmsUserDTO>();
                        }

                        cachedUsers = users;
                    }
                }
            }

            return GroupUsers(cachedUsers);
        }

        private static List<LmsUserDTO> CheckCachedUsers(LmsCourseMeeting meeting, bool forceUpdate, TimeSpan timeout)
        {
            return forceUpdate ? null : meeting.Return(x => x.CachedUsersParsed(timeout), null);
        }

        private static object GetLocker(string lockerKey)
        {
            if (!locker.ContainsKey(lockerKey))
            {
                lock (locker)
                {
                    if (!locker.ContainsKey(lockerKey))
                    {
                        locker.Add(lockerKey, new object());
                    }
                }
            }

            return locker[lockerKey];
        }
    }
}