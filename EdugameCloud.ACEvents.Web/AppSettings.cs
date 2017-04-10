using EdugameCloud.Certificates.Pdf;

namespace EdugameCloud.ACEvents.Web
{
    public class AppSettings
    {
        public CertificateSettings CertificateSettings { get; set; }

        public WebServiceReferenceSettings WebServiceReferences { get; set; }

        public string GoddardApi { get; set; }

        public class WebServiceReferenceSettings
        {
            public string EmailService { get; set; }
            public string CompanyEventsService { get; set; }
            public string LookupService { get; set; }
            public string QuizService { get; set; }
            public string QuizResultService { get; set; }
            public string FileService { get; set; }
            public string CompanyAcDomainsService { get; set; }
        }
    }
}