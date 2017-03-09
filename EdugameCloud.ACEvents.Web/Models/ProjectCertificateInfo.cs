using System;

namespace EdugameCloud.ACEvents.Web.Models
{
    public class ProjectCertificateInfo
    {
        public int ProjectUserId { get; set; }

        public Guid? CertificateTemplateContentId { get; set; }


        // QIUZ
        public int QuizId { get; set; }

        public QuizMode QuizMode { get; set; }

        public int PassingScore { get; set; }

        // QIUZ RESULT        
        public int? Score { get; set; }

        public bool? Completed { get; set; }

        public DateTime? CompletedTime { get; set; }

        // PROJECT USER
        public string FirstName { get; set; }

        public string LastName { get; set; }


        // PORTAL

        public string PortalName { get; set; }


        public string CompanyName { get; set; }

    }
}