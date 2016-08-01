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

        public const string ReportBulkUsers = "report-bulk-users";

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

        public const string ReportScoViews = "report-sco-views";

        internal static class Telephony
        {
            public const string ProviderList = "telephony-provider-list";

            public const string ProfileList = "telephony-profile-list";

            public const string ProfileInfo = "telephony-profile-info";

            public const string ProfileUpdate = "telephony-profile-update";

            public const string ProfileDelete = "telephony-profile-delete";

        }

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
            /// The expanded contents.
            /// </summary>
            public const string ExpandedContents = "sco-expanded-contents";

            /// <summary>
            /// The update.
            /// </summary>
            public const string Update = "sco-update";

            /// <summary>
            /// The update.
            /// </summary>
            public const string Upload = "sco-upload";

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
            /// The acl field info.
            /// </summary>
            public const string FieldInfo = "acl-field-info";

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
        /// The curriculum.
        /// </summary>
        internal static class Curriculum
        {
            /// <summary>
            /// The contents.
            /// </summary>
            public const string Contents = "curriculum-contents";

            /// <summary>
            /// The contents.
            /// </summary>
            public const string LearningPathInfo = "learning-path-info";

            /// <summary>
            /// The contents.
            /// </summary>
            public const string LearningPathUpdate = "learning-path-update";

            /// <summary>
            /// The report curriculum taker.
            /// </summary>
            public const string ReportCurriculumTaker = "report-curriculum-taker";
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
            /// The group membership update.
            /// </summary>
            public const string GroupMembershipUpdate = "group-membership-update";

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

        internal static class Seminar
        {
            public const string SeminarSessionScoUpdate = "seminar-session-sco-update";
            public const string SeminarLicensesList = "sco-seminar-licenses-list";
        }
    }
}
