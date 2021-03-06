﻿using System.Collections.Generic;
using System.Linq;
using Esynctraining.AC.Provider.Entities;

namespace Esynctraining.AC.Provider.DataObjects.Results
{
    public class PermissionCollectionResult : CollectionResult<PermissionInfo>
    {
        public PermissionCollectionResult(StatusInfo status) : base(status)
        {
        }

        public PermissionCollectionResult(StatusInfo status, IEnumerable<PermissionInfo> values) : base(status, values)
        {
        }

        internal MeetingPermissionCollectionResult ConvertForMeeting()
        {
            if (Status.Code != StatusCodes.ok)
                return new MeetingPermissionCollectionResult(Status);

            var meetingPermissions = Values?.Select(x => new MeetingPermissionInfo(x)).ToList();
            return new MeetingPermissionCollectionResult(Status, meetingPermissions);
        }

    }

    public class MeetingPermissionCollectionResult : CollectionResult<MeetingPermissionInfo>
    {
        public MeetingPermissionCollectionResult(StatusInfo status) : base(status)
        {
        }

        public MeetingPermissionCollectionResult(StatusInfo status, IEnumerable<MeetingPermissionInfo> values) : base(status, values)
        {
        }
    }

}
