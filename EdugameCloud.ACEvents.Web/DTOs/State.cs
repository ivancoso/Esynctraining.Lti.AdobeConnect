namespace EdugameCloud.ACEvents.Web.DTOs
{
    public class State
    {
        
        public int Id { get; set; }
        public string StateCode { get; set; }
        public string StateName { get; set; }
        public bool DynamicCourseNumber { get; set; }
    }

    public class Course
    {
        
        public int Id { get; set; }
        public string CourseId { get; set; }
        public string CourseTitle { get; set; }
    }

    public class CoreKnowledgeArea
    {
        
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StateId { get; set; }
        public string Name { get; set; }
    }

    public class Trainer
    {
        
        public int Id { get; set; }
        public string TrainerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class StateTrainerNumber
    {
        
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public int StateId { get; set; }
        public string Label { get; set; }

        public State State { get; set; }
        public Trainer Trainer { get; set; }
    }

    public class StateTrainerCourseNumber
    {
        
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int StateId { get; set; }
        public int TrainerId { get; set; }
        public string CourseTitle { get; set; }

        public Course Course { get; set; }
        public State State { get; set; }
        public Trainer Trainer { get; set; }
    }

    public class EventStateCourseNumber
    {
        
        public int Id { get; set; }
        public string EventId { get; set; }
        public int StateId { get; set; }
        public string Name { get; set; }

        public State State { get; set; }
    }

}