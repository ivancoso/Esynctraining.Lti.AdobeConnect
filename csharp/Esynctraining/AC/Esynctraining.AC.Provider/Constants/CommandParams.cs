﻿namespace Esynctraining.AC.Provider.Constants
{
    /// <summary>
    /// Adobe Connect API command parameters formatting strings.
    /// </summary>
    public static class CommandParams
    {

        /// <summary>
        /// The login
        /// </summary>
        public const string LoginParams = "login={0}&password={1}";

        /// <summary>
        /// The login
        /// </summary>
        public const string LoginWithAccountParams = "login={0}&password={1}&account-id={2}";

        /// <summary>
        /// The job id
        /// </summary>
        public const string JobId = "job-id={0}";

        /// <summary>
        /// The folder id
        /// </summary>
        public const string FolderId = "folder-id={0}";


        /// <summary>
        /// The SCO id.
        /// </summary>
        public const string ScoId = "sco-id={0}";

        /// <summary>
        /// Source sco id.
        /// </summary>
        public const string SourceScoId = "source-sco-id={0}";

        public const string ScoIdFilterSourceScoId = "sco-id={0}&filter-source-sco-id={1}";

        public const string ScoIdInteractionId = "sco-id={0}&interaction-id={1}";

        /// <summary>
        /// The SCO upload id.
        /// </summary>
        public const string ScoUpload = "sco-id={0}&summary={1}&title={2}";

        /// <summary>
        /// The curriculum id and SCO id.
        /// </summary>
        public const string CurriculumIdAndScoId = "curriculum-id={0}&sco-id={1}";

        /// <summary>
        /// The learning path update.
        /// </summary>
        public const string LearningPathUpdate = "curriculum-id={0}&current-sco-id={1}&target-sco-id={2}&path-type={3}";

        /// <summary>
        /// The SCO id and user id.
        /// </summary>
        public const string ScoIdAndUserId = "sco-id={0}&user-id={1}";

        /// <summary>
        /// The SCO id.
        /// </summary>
        public const string FieldAndQuery = "field={0}&query={1}";

        /// <summary>
        /// The SCO id.
        /// </summary>
        public const string Move = "folder-id={0}&sco-id={1}";

        /// <summary>
        /// The principal id public access
        /// </summary>
        public const string PrincipalId = "principal-id={0}";

        /// <summary>
        /// The principal id public access
        /// </summary>
        public const string PrincipalIdPublicAccess = "public-access";

        /// <summary>
        /// The principal group id users only
        /// </summary>
        public const string PrincipalGroupIdUsersOnly = "group-id={0}&filter-type=user&filter-is-member=true";

        public const string PrincipalGroupIdPrincipalId = "group-id={0}&filter-is-member=true&filter-principal-id={1}";

        /// <summary>
        /// The principal by email
        /// </summary>
        public const string PrincipalByEmail = "filter-email={0}&filter-is-primary=false";

        /// <summary>
        /// The principal by email and type
        /// </summary>
        public const string PrincipalByEmailAndType = "filter-email={0}&filter-type={1}";

        /// <summary>
        /// The principal by email
        /// </summary>
        public const string PrincipalByLogin = "filter-login={0}&filter-is-primary=false";

        public const string PrincipalByFieldLike = "filter-like-{0}={1}";

        public const string PrincipalByPrincipalId = "filter-principal-id={0}";


        /// <summary>
        /// The principal update password.
        /// </summary>
        public const string PrincipalUpdatePassword = "user-id={0}&password={1}&password-verify={2}";

        public const string PrincipalUpdateType = "principal-id={0}&type={1}";

        /// <summary>
        /// The report meeting attendance.
        /// </summary>
        //public const string ReportMeetingAttendance = "sco-id={0}";

        /// <summary>
        /// The meeting archives.
        /// </summary>
        public const string MeetingArchives = "sco-id={0}&filter-icon=archive";

        /// <summary>
        /// All archives including mp4.
        /// </summary>
        public const string MeetingArchivesWithMP4 = "sco-id={0}&filter-icon=archive&filter-icon=mp4-archive";

        /// <summary>
        /// The sco name.
        /// </summary>
        public const string FilterName = "filter-name={0}";

        public const string FilterIcon = "filter-icon={0}";

        public const string FilterType = "filter-type={0}";

        public const string FilterNameLike = "filter-like-name={0}";

        public const string ScoNameAndType = "sco-id={0}&filter-name={1}&filter-type={2}";

        /// <summary>
        /// The group membership.
        /// </summary>
        public const string GroupMembership = "group-id={0}&principal-id={1}&is-member={2}";

        /// <summary>
        /// The folder curriculum.
        /// </summary>
        public const string FolderCurriculums = "sco-id={0}&filter-type=curriculum";

        /// <summary>
        /// The meeting archives.
        /// </summary>
        public const string Meetings = "sco-id={0}&filter-type=meeting";

        /// <summary>
        /// The url path.
        /// </summary>
        public const string UrlPath = "url-path={0}";

        public static class Telephony
        {
            public const string ProfileId = "profile-id={0}";

            public const string UserConfiguredProfiles = "is-meeting-host=true&principal-id={0}";

        }

        public const string FolderIdAndSeminarSessionId = "folder-id={0}&seminar-session-id={1}";

        /// <summary>
        /// The report bulk consolidated transactions filters.
        /// </summary>
        internal static class ReportBulkConsolidatedTransactionsFilters
        {
            public const string TypeRecording = "filter-type=content";

            public const string AndByScoId = "&filter-sco-id={0}";

            public const string MeetingScoId = "filter-type=meeting&filter-sco-id={0}";

            public const string PrincipalId = "filter-type=meeting&filter-principal-id={0}&sort-date-created=desc";
        }

        /// <summary>
        /// The shortcut types.
        /// </summary>
        internal static class ShortcutTypes
        {
            /// <summary>
            /// The event.
            /// </summary>
            public const string Events = "events";

            /// <summary>
            /// The meetings.
            /// </summary>
            public const string Meetings = "meetings";
        }

        /// <summary>
        /// The report bulk objects filters.
        /// </summary>
        public static class ReportBulkObjectsFilters
        {
            /// <summary>
            /// Gets the meeting.
            /// </summary>
            public const string Meeting = "filter-type=meeting";

            public const string ByNameLike = "filter-like-name={0}";

            /// <summary>
            /// Gets the recordings.
            /// </summary>
            public const string Recording = "filter-icon=archive";
        }

        internal static class ReportBulkUsersFilters
        {
            //public const string Guest = "filter-type=guest";

            public const string GuestByLogin = "filter-type=guest&filter-login={0}";

            public const string GuestByEmail = "filter-type=guest&filter-email={0}";

        }

        /// <summary>
        /// The features.
        /// </summary>
        internal static class Features
        {
            public const string AccountId = "account-id={0}";
            /// <summary>
            /// The update.
            /// </summary>
            public const string Update = "account-id={0}&feature-id={1}&enable={2}";

            /// <summary>
            /// The field update.
            /// </summary>
            public const string FieldUpdate = "acl-id={0}&field-id={1}&value={2}";

            /// <summary>
            /// The field update with Required passcode.
            /// </summary>
            public const string FieldUpdateWithPasscode = "acl-id={0}&field-id={1}&value={2}&is-mtg-passcode-req={3}&permission-id=view-only&principal-id=public-access";

            /// <summary>
            /// The field info.
            /// </summary>
            public const string FieldInfo = "acl-id={0}&filter-field-id={1}";

            public const string FieldInfoAll = "acl-id={0}";
        }

        /// <summary>
        /// The permissions.
        /// </summary>
        internal static class Permissions
        {
            /// <summary>
            /// The permission update
            /// </summary>
            public const string Update = "acl-id={0}&principal-id={1}&permission-id={2}";

            /// <summary>
            /// The ACL id.
            /// </summary>
            public const string AclId = "acl-id={0}";

            /// <summary>
            /// The principal id.
            /// </summary>
            public const string PrincipalId = "principal-id={0}";

            /// <summary>
            /// The filter.
            /// </summary>
            internal static class Filter
            {
                /// <summary>
                /// The permission id.
                /// http://help.adobe.com/en_US/connect/8.0/webservices/WS8d7bb3e8da6fb92f73b3823d121e63182fe-8000_SP1.html#WS5b3ccc516d4fbf351e63e3d11a171ddf77-7fe9_SP1
                /// </summary>
                internal static class PermissionId
                {
                    public const string Format = "filter-permission-id={0}";

                    public const string MeetingAll = "filter-permission-id=host&filter-permission-id=mini-host&filter-permission-id=view";

                    public const string NonMeetingAll = "filter-permission-id=view&filter-permission-id=view-only&filter-permission-id=publish&filter-permission-id=manage&filter-permission-id=denied";
                }

            }

        }

        public static class CustomFields
        {
            public static readonly string FilterByName = "filter-name={0}";
            public static readonly string FilterObjectType = "filter-object-type={0}";
            public static readonly string FilterByNameAndObjectTypeFormat = "filter-name={0}&filter-object-type={1}";

            public static readonly string Delete = "field-id={0}&object-type={1}";

        }

        internal static class VirtualClassroom
        {
            public const string LicenseModelUpdate = "sco-id={0}&named-vc-model={1}";
        }
    }

}
