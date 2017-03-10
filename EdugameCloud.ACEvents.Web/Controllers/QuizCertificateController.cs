using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using EdugameCloud.ACEvents.Web.FileServiceNamespace;
using EdugameCloud.ACEvents.Web.Models;
using EdugameCloud.ACEvents.Web.QuizResultServiceNamespace;
using EdugameCloud.Certificates.Pdf;
using EdugameCloud.Core.Business.Models;
using Esynctraining.Core.Logging;
//using EdugameCloud.Certificates.Pdf;

//using SimuLyve.Mbm.Core.BusinessModels;
//using SimuLyve.Mbm.Core.BusinessModels.Pdf;
//using SimuLyve.Mbm.Core.DomailModel;

namespace EdugameCloud.ACEvents.Web.Controllers
{
    public partial class QuizCertificateController : Controller
    {
        private static readonly object _locker = new object();
        private readonly QuizResultModel _quizResultModel;
        private readonly QuizResultService _quizResultService;
        //private readonly FileModel _fileModel;


        public QuizCertificateController(QuizResultService quizResultService)
        {
            _quizResultService = quizResultService;
        }


        [OutputCache(CacheProfile = "QuizPreview")]
        public virtual ActionResult Preview(Guid quizResultGuid)
        {
            try
            {
                ProjectCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return HttpNotFound();

                string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Jpeg, BuildTemplateData(quizResult));
                return File(imagePath, "image/jpeg", "CertificatePreview.jpg");

            }
            catch (Exception ex)
            {
                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
                throw;
            }
        }

        [OutputCache(CacheProfile = "QuizDownload")]
        public virtual ActionResult Download(Guid quizResultGuid)
        {
            try
            {
                ProjectCertificateInfo quizResult = FindCompletedQuizResult(quizResultGuid);
                if (quizResult == null)
                    return HttpNotFound();

                if (Request.Browser.IsMobileDevice)
                {
                    string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Png, BuildTemplateData(quizResult), resize: false);
                    return File(imagePath, "image/png", "Certificate.png");
                }

                string filePath = GetCertProcessor(quizResult).RenderPdfDocument(BuildTemplateData(quizResult));
                return File(filePath, "application/pdf", "Certificate.pdf");
            }
            catch (Exception ex)
            {
                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
                throw;
            }
        }


        private QuizCerfificateProcessor GetCertProcessor(ProjectCertificateInfo value)
        {
            string certificateTemplateFilePath = GetCertificatePath(value.CertificateTemplateContentId.Value);
            return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateTemplateContentId.Value.ToString());
        }

        //private  GetCertProcessor(ProjectCertificateInfo value)
        //{
        //    string certificateTemplateFilePath = GetCertificatePath(value.CertificateTemplateContentId.Value);
        //    return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateTemplateContentId.Value.ToString());
        //}

        private string GetCertificatePath(Guid certificateTemplateContentId)
        {
            string certificateTemplateFilePath = GetPdfTempatePath(certificateTemplateContentId);

            // TODO: think about by-key locking
            if (!System.IO.File.Exists(certificateTemplateFilePath))
            {
                lock (_locker)
                {
                    if (!System.IO.File.Exists(certificateTemplateFilePath))
                    {
                        //var certTemplate = _fileModel.GetOneById(certificateTemplateContentId).Value;
                        //var certTemplate = _fileModel.GetOneById(certificateTemplateContentId).Value;
                        var certTemplate = new FileDTO()
                        {
                            fileName = "temp"
                        };
                        var content = System.IO.File.ReadAllBytes(certTemplate.fileName);
                        System.IO.File.WriteAllBytes(certificateTemplateFilePath, content);
                    }
                }
            }

            return certificateTemplateFilePath;
        }

        private static string GetPdfTempatePath(Guid certificateTemplateContentId)
        {
            string setting = ConfigurationManager.AppSettings["PdfTemplateFolder"];
            string folder = setting.StartsWith("~")
                ? HostingEnvironment.MapPath(setting)
                : setting;

            return Path.Combine(folder,
                string.Format("{0}.pdf", certificateTemplateContentId.ToString()));
        }

        private ProjectCertificateInfo FindCompletedQuizResult(Guid quizResultGuid)
        {
            //if ((projectId <= 0) || (token == Guid.Empty))
            //    return null;

            //AccountSession session = _sessionModel.GetParticipantByToken(token);
            //if (session == null)
            //    return null;

            //return _projectModel.FindInfoByProjectUser(projectId, session.AccountId);
            return new ProjectCertificateInfo()
            {
                CertificateTemplateContentId = Guid.Parse("D97EAEEC-398A-47DC-B6DE-408A2E89F9E2"),
                CompanyName = "Goddard",
                FirstName = "FirstName",
                LastName = "LastName",
                PassingScore = 90,
                Score = 100,
                PortalName = "Quiz Portal",
                Completed = true,
                CompletedTime = DateTime.Now,
                QuizMode = QuizMode.Enabled,
            };
        }


        private static Dictionary<string, string> BuildTemplateData(ProjectCertificateInfo value)
        {
            var fields = new Dictionary<string, string>
                {
                    { "ParticipantId", value.ProjectUserId.ToString() },
                    { "FullName", string.Format("{0} {1}", value.FirstName, value.LastName) },
                    { "PortalName", value.PortalName },
                    { "CompanyName", value.CompanyName },
                    { "CompletedDate",  value.CompletedTime.HasValue ? value.CompletedTime.Value.ToUniversalTime().ToString("MMMM dd, yyyy") : string.Empty },
                    { "Score", value.Score.ToString() },
                };

            return fields;
        }

    }

}