namespace Esynctraining.AC.Provider.Constants
{
    /// <summary>
    /// Adobe Connect API commands
    /// </summary>
    internal static class Commands
    {
        /// <summary>
        /// The login
        /// </summary>
        public const string Login = "login";

        /// <summary>
        /// The logout
        /// </summary>
        public const string Logout = "logout";

        /// <summary>
        /// The common info.
        /// </summary>
        public const string CommonInfo = "common-info";

        /// <summary>
        /// The report bulk objects.
        /// </summary>
        public const string ReportBulkObjects = "report-bulk-objects";

        /// <summary>
        /// The report bulk attendance.
        /// </summary>
        public const string ReportMeetingAttendance = "report-meeting-attendance";

        /// <summary>
        /// The report bulk meeting session.
        /// </summary>
        public const string ReportMeetingSessions = "report-meeting-sessions";

        /// <summary>
        /// The report my events.
        /// </summary>
        public const string ReportMyEvents = "report-my-events";

        /// <summary>
        /// The report my events.
        /// </summary>
        public const string ReportMyMeetings = "report-my-meetings";

        /// <summary>
        /// The report bulk consolidated transactions.
        /// </summary>
        public const string ReportBulkConsolidatedTransactions = "report-bulk-consolidated-transactions";

        /// <summary>
        /// The report quiz interactions.
        /// </summary>
        public const string ReportQuizInteractions = "report-quiz-interactions";

        /// <summary>
        /// The telephony profile list.
        /// </summary>
        public const string TelephonyProfileList = "telephony-profile-list";

        /// <summary>
        /// SCO commands.
        /// </summary>
        internal static class Sco
        {
            /// <summary>
            /// The shortcuts.
            /// </summary>
            public const string Shortcuts = "sco-shortcuts";

            /// <summary>
            /// The contents.
            /// </summary>
            public const string Contents = "sco-contents";

            /// <summary>
            /// The info.
            /// </summary>
            public const string Info = "sco-info";

            /// <summary>
            /// The update.
            /// </summary>
            public const string Update = "sco-update";

            /// <summary>
            /// The delete.
            /// </summary>
            public const string Delete = "sco-delete";

            /// <summary>
            /// The delete.
            /// </summary>
            public const string Move = "sco-move";

            /// <summary>
            /// The meeting feature update.
            /// </summary>
            public const string FeatureUpdate = "meeting-feature-update";

            /// <summary>
            /// The acl field update.
            /// </summary>
            public const string FieldUpdate = "acl-field-update";

            /// <summary>
            /// The update.
            /// </summary>
            public const string ByUrl = "sco-by-url";

            /// <summary>
            /// The search by field.
            /// </summary>
            public const string SearchByField = "sco-search-by-field";
        }

        /// <summary>
        /// Permission commands.
        /// </summary>
        internal static class Permissions
        {
            /// <summary>
            /// The permission update.
            /// </summary>
            public const string Update = "permissions-update";

            /// <summary>
            /// The permission info.
            /// </summary>
            public const string Info = "permissions-info";
        }

        /// <summary>
        /// The principal.
        /// </summary>
        internal static class Principal
        {
            /// <summary>
            /// The info.
            /// </summary>
            public const string Info = "principal-info";

            /// <summary>
            /// The list.
            /// </summary>
            public const string List = "principal-list";

            /// <summary>
            /// The update.
            /// </summary>
            public const string Update = "principal-update";

            /// <summary>
            /// The update.
            /// </summary>
            public const string UpdatePassword = "user-update-pwd";

            /// <summary>
            /// The update.
            /// </summary>
            public const string Delete = "principals-delete";
        }

        /// <summary>
        /// Meeting recordings commands
        /// </summary>
        internal static class Recordings
        {
            /// <summary>
            /// Lists conversion jobs.
            /// </summary>
            public const string ListJobs = "list-recording-jobs";

            /// <summary>
            /// Schedules a new conversion job.
            /// </summary>
            public const string ScheduleJob = "schedule-recording-job";

            /// <summary>
            /// Gets conversion job details.
            /// </summary>
            public const string GetJob = "get-recording-job";

            /// <summary>
            /// Cancels conversion job.
            /// </summary>
            public const string CancelJob = "cancel-recording-job";

            /// <summary>
            /// Lists meeting recordings (archived meetings).
            /// </summary>
            public const string List = "list-recordings";

            /// <summary>
            /// Lists recordings converted to video files (FLV or MP4).
            /// </summary>
            public const string ListConverted = "list-generated-recordings";
        }
    }
}
