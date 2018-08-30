﻿using Esynctraining.Lti.Lms.Common.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.BlackBoardClient;
using Esynctraining.Core.Domain;
using Esynctraining.Core.Logging;
using Esynctraining.Lti.Lms.Common.API.BlackBoard;
using Esynctraining.Lti.Lms.Common.Dto;

namespace Esynctraining.Lti.Lms.BlackBoard
{
    //public class BlackboardLmsUserService : LmsUserServiceBase
    //{
    //    private readonly IBlackBoardApi _soapApi;


    //    public BlackboardLmsUserService(ILogger logger, IBlackBoardApi soapApi) : base(logger)
    //    {
    //        _soapApi = soapApi ?? throw new ArgumentNullException(nameof(soapApi));
    //    }


    //    //public override async Task<(LmsUserDTO user, string error)> GetUser(Dictionary<string, object> licenseSettings, string lmsUserId, string courseId, LtiParamDTO extraData = null)
    //    //{
    //    //    Guid guid;
    //    //    //return GetUsersOldStyle(lmsCompany, courseId, out error)
    //    //    //    .FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id));

    //    //    var result = await GetUsersOldStyle(licenseSettings, courseId, extraData);
    //    //    if (string.IsNullOrWhiteSpace(result.error))
    //    //        return (result.users.FirstOrDefault(u => lmsUserId == (Guid.TryParse(lmsUserId, out guid) ? u.LtiId : u.Id)), result.error);

    //    //    return (null, result.error);
    //    //}

    //    public override async Task<OperationResultWithData<List<LmsUserDTO>>> GetUsers(Dictionary<string, object> licenseSettings,
    //        string courseId, LtiParamDTO extraData = null)
    //    {
    //        if (licenseSettings == null)
    //            throw new ArgumentNullException(nameof(licenseSettings));

    //        var users = await GetUsersOldStyle(licenseSettings, courseId, extraData);
    //        return users.Item1.ToSuccessResult();
    //    }

    //    public override Task<(List<LmsUserDTO> users, string error)> GetUsersOldStyle(Dictionary<string, object> licenseSettings,
    //        string courseId, LtiParamDTO param = null)
    //    {
    //        if (licenseSettings == null)
    //            throw new ArgumentNullException(nameof(licenseSettings));

    //        string error = null;
    //        string[] userIds = null;

    //        WebserviceWrapper client = null;
    //        List<LmsUserDTO> users = _soapApi.GetUsersForCourse(
    //            licenseSettings,
    //            courseId,
    //            userIds,
    //            out error,
    //            ref client);

    //        if ((users.Count == 0)
    //            && error.Return(
    //                x => x.IndexOf("ACCESS DENIED", StringComparison.InvariantCultureIgnoreCase) >= 0,
    //                false))
    //        {
    //            Logger.Warn("GetBlackBoardUsers.AccessDenied. " + error);

    //            if (client != null)
    //                client.logout();
    //            // NOTE: set to null to re-create session.
    //            client = null;
    //            users = _soapApi.GetUsersForCourse(
    //                licenseSettings,
    //                courseId,
    //                userIds,
    //                out error,
    //                ref client);
    //        }

    //        // NOTE: always call logout
    //        if (client != null)
    //            client.logout();

    //        return Task.FromResult<(List<LmsUserDTO> users, string error)>((GroupUsers(users), error));
    //    }

    //}
}
