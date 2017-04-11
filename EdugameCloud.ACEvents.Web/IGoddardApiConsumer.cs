using EdugameCloud.ACEvents.Web.DTOs;

namespace EdugameCloud.ACEvents.Web
{
    public interface IGoddardApiConsumer
    {
        Trainer GetTrainer(string trainerId);
        CoreKnowledgeArea GetCoreKnowledgeArea(string courseId, string stateCode);
        StateTrainerNumber GetStateTrainerNumber(string trainerId, string stateCode);
        StateTrainerCourseNumber GetStateTrainerCourseNumber(string courseId, string stateCode, string trainerId);
        EventStateCourseNumber GetEventStateCourseNumber(string eventId, string stateCode);
        Course GetCourse(string courseId);
    }
}