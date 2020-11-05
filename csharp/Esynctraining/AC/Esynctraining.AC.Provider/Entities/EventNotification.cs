using System;

namespace Esynctraining.AC.Provider.Entities
{
    /*
<notification target-acl-id="2155548270" action-id="2155538787" action-type-id="notification" 
    status="aborted" template-id="event-absentee-follow-up" zone-id="33">
<date-modified>2019-06-11T02:20:04.823-07:00</date-modified>
<date-scheduled>2019-04-11T10:15:00.000-07:00</date-scheduled>
<notification-sent>false</notification-sent>
<cq-template-name>Absentee Follow Up</cq-template-name>
<cq-template-path>en/events/email/shared/absentee</cq-template-path>
<email-subject>{event-name} Follow-Up</email-subject>
<relative-date>12-hours-after</relative-date>
<cq-email-template>1248852437</cq-email-template>
</notification>

    */
    public class EventNotification
    {
        public static class Statuses
        {
            public static readonly string Completed = "completed";
            public static readonly string Aborted = "aborted";
            public static readonly string InProgress = "in-progress";
            public static readonly string Retry = "retry";
            public static readonly string New = "new";
            public static readonly string Error = "error";
        }

        public static class Templates
        {
            public static readonly string PermissionApproved = "permission-approved";
            public static readonly string EventAbsenteeFollowUp = "event-absentee-follow-up";
            public static readonly string ScoView = "sco-view";
            public static readonly string EventReminder = "event-reminder";
            public static readonly string EventThankYou = "event-thank-you";
            public static readonly string EventUpdate = "event-update";
            public static readonly string PermissionPending = "permission-pending";
            public static readonly string PermissionDenied = "permission-denied";
            public static readonly string EventInvite = "event-invite";
            public static readonly string EventCustom = "event-custom";
        }


        public string TargetAclId { get; set; }

        public string ActionId { get; set; }

        public string ActionTypeId { get; set; }

        /// <summary>
        /// Status of an action. Most common values are completed, aborted, in-progress, retry, new, and error
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// permission-approved
        /// event-absentee-follow-up
        /// sco-view
        /// event-reminder
        /// event-thank-you
        /// event-update
        /// permission-pending
        /// permission-denied
        /// event-invite
        /// event-custom (SEVERAL ENTRIES are possible)
        /// </summary>
        public string TemplateId { get; set; }

        public string ZoneId { get; set; }

        
        public DateTime DateModified { get; set; }

        public DateTime DateScheduled { get; set; }

        public bool NotificationSent { get; set; }

        public string CqTemplateName { get; set; }

        public string CqTemplatePath { get; set; }

        public string EmailSubject { get; set; }

        public string RelativeDate { get; set; }

        public string CqEmailTemplate { get; set; }

    }

}
