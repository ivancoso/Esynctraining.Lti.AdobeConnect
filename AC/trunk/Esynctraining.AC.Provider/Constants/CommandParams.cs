namespace Esynctraining.AC.Provider.Constants
{
    /// <summary>
    /// Adobe Connect API command parameters formatting strings.
    /// </summary>
    internal static class CommandParams
    {
        /// <summary>
        /// The login
        /// </summary>
        public const string LoginParams = "login={0}&password={1}";

        /// <summary>
        /// The SCO id.
        /// </summary>
        public const string ScoId = "sco-id={0}";

        /// <summary>
        /// The SCO id.
        /// </summary>
        public const string Move = "folder-id={0}&sco-id={1}";

        /// <summary>
        /// The principal id public access
        /// </summary>
        public const string PrincipalIdPublicAccess = "public-access";

        /// <summary>
        /// The principal group id users only
        /// </summary>
        public const string PrincipalGroupIdUsersOnly = "group-id={0}&filter-type=user&sort-name=asc";

        /// <summary>
        /// The principal by email
        /// </summary>
        public const string PrincipalByEmail = "filter-email={0}";

        /// <summary>
        /// The principal update password.
        /// </summary>
        public const string PrincipalUpdatePassword = "user-id={0}&password={1}&password-verify={2}";

        /// <summary>
        /// The report meeting attendance.
        /// </summary>
        public const string ReportMeetingAttendance = "sco-id={0}";

        /// <summary>
        /// The meeting archives.
        /// </summary>
        public const string MeetingArchives = "sco-id={0}&filter-icon=archive";

        /// <summary>
        /// The url path.
        /// </summary>
        public const string UrlPath = "url-path={0}";

        /// <summary>
        /// The report bulk consolidated transactions filters.
        /// </summary>
        internal static class ReportBulkConsolidatedTransactionsFilters
        {
            /// <summary>
            /// The meeting SCO id.
            /// </summary>
            public const string MeetingScoId = "filter-type=meeting&filter-sco-id={0}";            
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
        internal static class ReportBulkObjectsFilters
        {
            /// <summary>
            /// Gets the meeting.
            /// </summary>
            public const string Meeting = "filter-type=meeting";

            /// <summary>
            /// Gets the recordings.
            /// </summary>
            public const string Recording = "filter-icon=archive";
        }

        /// <summary>
        /// The features.
        /// </summary>
        internal static class Features
        {
            /// <summary>
            /// The update.
            /// </summary>
            public const string Update = "account-id={0}&feature-id={1}&enable={2}";

            /// <summary>
            /// The field update.
            /// </summary>
            public const string FieldUpdate = "acl-id={0}&field-id={1}&value={2}";
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
                /// </summary>
                internal static class PermissionId
                {
                    /// <summary>
                    /// The host.
                    /// </summary>
                    public const string Host = "filter-permission-id=host";

                    /// <summary>
                    /// The mini host.
                    /// </summary>
                    public const string MiniHost = "filter-permission-id=mini-host";

                    /// <summary>
                    /// The view.
                    /// </summary>
                    public const string View = "filter-permission-id=view";
                }
            }
        }
    }
}
