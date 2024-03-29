﻿using EdugameCloud.Lti.Domain.Entities;
using Esynctraining.AC.Provider.Entities;

namespace EdugameCloud.Lti.API.AdobeConnect
{
    public static class MeetingTypeFactory
    {
        public static PrincipalType HostGroup(LmsMeetingType meetingType)
        {
            switch (meetingType)
            {
                case LmsMeetingType.VirtualClassroom:
                    return PrincipalType.named_vc_admins;
                default:
                    return PrincipalType.live_admins;

            }
        }

        public static ScoShortcutType GetMeetingFolderShortcut(LmsMeetingType meetingType, bool isUserFolder)
        {
            switch (meetingType)
            {
                case LmsMeetingType.VirtualClassroom:
                    return isUserFolder ? ScoShortcutType.user_courses : ScoShortcutType.courses;
                default:
                    return isUserFolder ? ScoShortcutType.user_meetings : ScoShortcutType.meetings;

            }
        }

        public static ScoShortcutType GetTemplatesShortcut(LmsMeetingType meetingType)
        {
            switch (meetingType)
            {
                case LmsMeetingType.VirtualClassroom:
                    return ScoShortcutType.shared_training_templates;
                default:
                    return ScoShortcutType.shared_meeting_templates;

            }
        }

        public static bool UseDbMeetingFolderId(LmsMeetingType meetingType)
        {
            switch (meetingType)
            {
                case LmsMeetingType.VirtualClassroom:
                    return false;
                default:
                    return true;

            }
        }
    }
}
