namespace Esynctraining.Lti.Lms.Canvas.DTOs
{
    /// <summary>
    /// https://canvas.instructure.com/doc/api/calendar_events.html
    /// </summary>
    public class CanvasCalendarEventDTO
    {
        public int id { get; set; }
        public string title { get; set; }
        public string start_at { get; set; }
        public string end_at { get; set; }
        public string description { get; set; }
        public string location_name { get; set; }
        public string location_address { get; set; }

        // the context code of the calendar this event belongs to (course, user or
        // group)
        public string context_code { get; set; }

        // if specified, it indicates which calendar this event should be displayed on.
        // for example, a section-level event would have the course's context code here,
        // while the section's context code would be returned above)
        public string effective_context_code { get; set; }

        // a comma-separated list of all calendar contexts this event is part of
        public string all_context_codes { get; set; }

        // Current state of the event ('active', 'locked' or 'deleted') 'locked'
        // indicates that start_at/end_at cannot be changed (though the event could be
        // deleted). Normally only reservations or time slots with reservations are
        // locked (see the Appointment Groups API)
        public string workflow_state { get; set; }

        // Whether this event should be displayed on the calendar. Only true for
        // course-level events with section-level child events.
        public bool hidden { get; set; }

        // Normally null. If this is a reservation (see the Appointment Groups API), the
        // id will indicate the time slot it is for. If this is a section-level event,
        // this will be the course-level parent event.
        public int? parent_event_id { get; set; }

        // The number of child_events. See child_events (and parent_event_id)
        public int child_events_count { get; set; }

        // Included by default, but may be excluded (see include[] option). If this is a
        // time slot (see the Appointment Groups API) this will be a list of any
        // reservations. If this is a course-level event, this will be a list of
        // section-level events (if any)
        public string child_events { get; set; }

        // URL for this calendar event (to update, delete, etc.)
        public string url { get; set; }

        // URL for a user to view this event
        public string html_url { get; set; }

        // The date of this event
        public string all_day_date { get; set; }

        // Boolean indicating whether this is an all-day event (midnight to midnight)
        public bool all_day { get; set; }

        // When the calendar event was created
        public string created_at { get; set; }

        // When the calendar event was last updated
        public string updated_at { get; set; }

        // Various Appointment-Group-related fields.These fields are only pertinent to
        // time slots (appointments) and reservations of those time slots. See the
        // Appointment Groups API. The id of the appointment group
        public string appointment_group_id { get; set; }

        // The API URL of the appointment group
        public string appointment_group_url { get; set; }

        // If the event is a reservation, this a boolean indicating whether it is the
        // current user's reservation, or someone else's
        public bool own_reservation { get; set; }

        // If the event is a time slot, the API URL for reserving it
        public string reserve_url { get; set; }

        // If the event is a time slot, a boolean indicating whether the user has
        // already made a reservation for it
        public bool reserved { get; set; }

        // The type of participant to sign up for a slot: 'User' or 'Group'
        public string participant_type { get; set; }

        // If the event is a time slot, this is the participant limit
        public string participants_per_appointment { get; set; }

        // If the event is a time slot and it has a participant limit, an integer
        // indicating how many slots are available
        public string available_slots { get; set; }

        // If the event is a user-level reservation, this will contain the user
        // participant JSON (refer to the Users API).
        public string user { get; set; }

        // If the event is a group-level reservation, this will contain the group
        // participant JSON (refer to the Groups API).
        public string group { get; set; }
    }
}
