namespace Esynctraining.Zoom.ApiWrapper.Model
{
    public enum MeetingTypes
    {
        Instant = 1,
        Scheduled = 2,
        RecurringNoTime = 3,
        RecurringWithTime = 4,
    }

    public enum MeetingApprovalTypes
    {
        Automatic,
        Manual,
        NoRegistration,
    }

    public enum MeetingRegistrationTypes
    {
        RegisterAllOccurrences = 1,
        RegisterEachOccurrence = 2,
        RegisterChooseOccurrence = 3,
    }


    public enum UserTypes
    {
        Basic = 1,
        Pro = 2,
        Corporate = 3,
    }

    public static class CreateUserAction
    {
        public static string Create = "create";
        public static string AutoCreate = "autoCreate";
        public static string CustCreate = "custCreate";
        public static string SsoCreate = "ssoCreate";
    }

    public enum UserStatus
    {
        Active,
        Inactive,
        Pending,
    }

    public enum MeetingListTypes
    {
        Scheduled,
        Live,
    }

    public static class MeetingAudioOptions
    {
        public static string Both = "both";
        public static string Telephone = "telephony";
        public static string Voip = "voip";
    }
}