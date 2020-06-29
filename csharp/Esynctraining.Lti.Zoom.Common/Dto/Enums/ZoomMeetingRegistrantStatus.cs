namespace Esynctraining.Lti.Zoom.Common.Dto.Enums
{
    public enum ZoomMeetingRegistrantStatus
    {
        Approval, // fake from api for "pending" request
        Approved,
        Pending,
        Denied,
        All // fake from api for "denied" request
    }

    public enum ZoomUserStatus
    {
        Active,
        Inactive,
        Pending,
    }
}