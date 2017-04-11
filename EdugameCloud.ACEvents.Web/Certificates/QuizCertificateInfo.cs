using System;

namespace EdugameCloud.ACEvents.Web.Certificates
{
    public class QuizCertificateInfo
    {
       
        public string ParticipantName { get; set; }
        public string CourseTitle { get; set; }
        public string KnowledgeArea { get; set; }   
        public string Duration { get; set; }
        public string CtoNumber { get; set; }
        public string Signature { get; set; }
        public string TrainerName { get; set; }
        public string Date { get; set; }
        public string ExpiresDate { get; set; }
        public string SpecificId { get; set; }
        public Guid CertificateTemplateGuid { get; set; }
        //public string State { get; set; }
        //public string School { get; set; }
        public string EventApprovalCode { get; set; }
        public string StateTrainerNumber { get; set; }
        public string StateTrainerCourseNumber { get; set; }

    }
   
}