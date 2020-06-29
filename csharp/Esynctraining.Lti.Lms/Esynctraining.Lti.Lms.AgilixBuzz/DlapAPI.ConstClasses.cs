namespace Esynctraining.Lti.Lms.AgilixBuzz
{
    public sealed partial class DlapAPI
    {
        private static class Commands
        {
            public static class Users
            {
                public const string GetOne = "getuser2";
            }

            public static class Enrollments
            {
                public const string List = "listenrollments";

                public const string GetOne = "getenrollment3";

            }

            public static class Signals
            {
                public const string List = "getsignallist";

                public const string GetLastSignalId = "getlastsignalid";
            }

            public static class Groups
            {
                public const string List = "getgrouplist";

            }


            public static class Courses
            {
                public const string GetOne = "getcourse2";

            }

            public static class Announcements
            {
                public const string Put = "putannouncement";

            }

        }

        private static class Parameters
        {
            public static class Users
            {
                public const string GetOne = "userid={0}";
            }

            public static class Enrollments
            {
                public const string List = "domainid={0}&limit=0&coursequery=%2Fid%3D{1}&select=user";

                public const string GetOne = "enrollmentid={0}&select=user";

            }

            public static class Signals
            {
                public const string List = "lastsignalid={0}&domainid={1}&type={2}";

            }

            public static class Courses
            {
                public const string GetOne = "courseid={0}";

            }

            public static class Groups
            {
                public const string List = "ownerid={0}";

            }

            public static class Announcements
            {
                public const string Put = "entityid={0}&path={1}";

            }

        }

        private static class Roles
        {
            public const string Author = "author";

            public const string Owner = "owner";

            public const string Reader = "reader";

            public const string Student = "student";

            public const string Teacher = "teacher";

        }

    }
}
