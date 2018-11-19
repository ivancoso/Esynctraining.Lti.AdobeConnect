namespace Esynctraining.Lti.Lms.Canvas.DTOs
{
    public class CanvasAssignmentEventDTO
    {
        // A synthetic ID for the assignment
        public string id { get; set; }

        // The title of the assignment
        public string title { get; set; }

        // The due_at timestamp of the assignment
        public string start_at { get; set; }

        // The due_at timestamp of the assignment
        public string end_at { get; set; }

        // The HTML description of the assignment
        public string description { get; set; }

        // the context code of the (course) calendar this assignment belongs to
        public string context_code { get; set; }

        // Current state of the assignment ('published' or 'deleted')
        public string workflow_state { get; set; }

        // URL for this assignment (note that updating/deleting should be done via the
        // Assignments API)
        public string url { get; set; }

        // URL for a user to view this assignment
        public string html_url { get; set; }

        // The due date of this assignment
        public string all_day_date { get; set; }

        // Boolean indicating whether this is an all-day event (e.g. assignment due at
        // midnight)
        public bool all_day { get; set; }

        // When the assignment was created
        public string created_at { get; set; }

        // When the assignment was last updated
        public string updated_at { get; set; }

        // The full assignment JSON data (See the Assignments API)
        public string assignment { get; set; }

        // The list of AssignmentOverrides that apply to this event (See the Assignments
        // API). This information is useful for determining which students or sections
        // this assignment-due event applies to.
        public string assignment_overrides { get; set; }
    }
}
