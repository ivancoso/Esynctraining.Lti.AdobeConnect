//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Drawing.Imaging;
//using System.IO;
//using System.Net.Mime;
//using System.Web;
//using System.Web.Hosting;
//using System.Web.Mvc;
//using EdugameCloud.ACEvents.Web.Models;
////using EdugameCloud.Certificates.Pdf;
//using EdugameCloud.Core.Business.Models;
//using Esynctraining.Core.Logging;
////using SimuLyve.Mbm.Core.BusinessModels;
////using SimuLyve.Mbm.Core.BusinessModels.Pdf;
////using SimuLyve.Mbm.Core.DomailModel;

//namespace SimuLyve.Mbm.Web.Controllers
//{
//    public partial class QuizCertificateController : Controller
//    {
//        private static readonly object _locker = new object();
//        private readonly QuizResultModel _quizResultModel;
//        private readonly FileModel _fileModel;


//        public QuizCertificateController(QuizResultModel quizResultModel, FileModel fileModel)
//        {
//            _fileModel = fileModel;
//            _quizResultModel = quizResultModel;
//        }


//        [OutputCache(CacheProfile = "QuizPreview")]
//        public virtual ActionResult Preview(int projectId, Guid token)
//        {
//            try
//            {
//                ProjectCertificateInfo quizResult = FindCompletedQuizResult(projectId, token);
//                if (quizResult == null)
//                    return HttpNotFound();

//                string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Jpeg, BuildTemplateData(quizResult));
//                return File(imagePath, "image/jpeg", "CertificatePreview.jpg");

//            }
//            catch (Exception ex)
//            {
//                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
//                throw;
//            }
//        }

//        [OutputCache(CacheProfile = "QuizDownload")]
//        public virtual ActionResult Download(int projectId, Guid token)
//        {
//            try
//            {
//                ProjectCertificateInfo quizResult = FindCompletedQuizResult(projectId, token);
//                if (quizResult == null)
//                    return HttpNotFound();

//                if (Request.Browser.IsMobileDevice)
//                {
//                    string imagePath = GetCertProcessor(quizResult).RenderPreview(ImageFormat.Png, BuildTemplateData(quizResult), resize: false);
//                    return File(imagePath, "image/png", "Certificate.png");
//                }

//                string filePath = GetCertProcessor(quizResult).RenderPdfDocument(BuildTemplateData(quizResult));
//                return File(filePath, "application/pdf", "Certificate.pdf");
//            }
//            catch (Exception ex)
//            {
//                Esynctraining.Core.Utils.IoC.Resolve<ILogger>().Error("Error during Certificate Preview.", ex);
//                throw;
//            }
//        }


//        //private QuizCerfificateProcessor GetCertProcessor(ProjectCertificateInfo value)
//        //{
//        //    string certificateTemplateFilePath = GetCertificatePath(value.CertificateTemplateContentId.Value);
//        //    return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateTemplateContentId.Value.ToString());
//        //}

//        private  GetCertProcessor(ProjectCertificateInfo value)
//        {
//            string certificateTemplateFilePath = GetCertificatePath(value.CertificateTemplateContentId.Value);
//            return new QuizCerfificateProcessor(certificateTemplateFilePath, value.CertificateTemplateContentId.Value.ToString());
//        }

//        private string GetCertificatePath(Guid certificateTemplateContentId)
//        {
//            string certificateTemplateFilePath = GetPdfTempatePath(certificateTemplateContentId);

//            // TODO: think about by-key locking
//            if (!System.IO.File.Exists(certificateTemplateFilePath))
//            {
//                lock (_locker)
//                {
//                    if (!System.IO.File.Exists(certificateTemplateFilePath))
//                    {
//                        var certTemplate = _fileModel.GetOneById(certificateTemplateContentId).Value;
//                        var content = System.IO.File.ReadAllBytes(certTemplate.FileName);
//                        System.IO.File.WriteAllBytes(certificateTemplateFilePath, content);
//                    }
//                }
//            }
            
//            return certificateTemplateFilePath;
//        }

//        private static string GetPdfTempatePath(Guid certificateTemplateContentId)
//        {
//            string setting = ConfigurationManager.AppSettings["PdfTemplateFolder"];
//            string folder = setting.StartsWith("~")
//                ? HostingEnvironment.MapPath(setting)
//                : setting;

//            return Path.Combine(folder,
//                string.Format("{0}.pdf", certificateTemplateContentId.ToString()));
//        }

//        private ProjectCertificateInfo FindCompletedQuizResult(int projectId, Guid token)
//        {
//            if ((projectId <= 0) || (token == Guid.Empty))
//                return null;

//            //AccountSession session = _sessionModel.GetParticipantByToken(token);
//            //if (session == null)
//            //    return null;

//            //return _projectModel.FindInfoByProjectUser(projectId, session.AccountId);
//            return null;
//        }


//        private static Dictionary<string, string> BuildTemplateData(ProjectCertificateInfo value)
//        {
//            var fields = new Dictionary<string, string>
//                {
//                    { "ParticipantId", value.ProjectUserId.ToString() },
//                    { "FullName", string.Format("{0} {1}", value.FirstName, value.LastName) },
//                    { "PortalName", value.PortalName },
//                    { "CompanyName", value.CompanyName },
//                    { "CompletedDate",  value.CompletedTime.HasValue ? value.CompletedTime.Value.ToUniversalTime().ToString("MMMM dd, yyyy") : string.Empty },
//                    { "Score", value.Score.ToString() },
//                };

//            return fields;
//        }

//    }

//}