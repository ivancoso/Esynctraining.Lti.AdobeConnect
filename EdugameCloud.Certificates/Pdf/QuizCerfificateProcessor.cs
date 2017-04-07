using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Esynctraining.PdfProcessor.Renders;
using iTextSharp.text.pdf;

namespace EdugameCloud.Certificates.Pdf
{
    public sealed class QuizCerfificateProcessor : IQuizCerfificateProcessor
    {
        private readonly CertificateSettings _certificateSettings;

        public QuizCerfificateProcessor(CertificateSettings settings)
        {
            Check.Argument.IsNotNullOrEmpty(settings.PdfOutputFolder, "PdfOutputFolder");
            Check.Argument.IsNotNullOrEmpty(settings.PdfTemplateFolder, "PdfTemplateFolder");
            _certificateSettings = settings;
        }

        //string certificateUid, string templateToken,
        public string RenderPdfDocument(string certificateUid, string templateUid, IDictionary<string, string> fields)
        {
            Check.Argument.IsNotNull(fields, "fields");
            Check.Argument.IsNotNull(fields, "certificateUid");

            string outputFilePath = BuildPdfOutputPath(certificateUid, fields["ParticipantName"]);
            var templateFilePath = GetPdfTempatePath(Guid.Parse(templateUid));
            RenderPdfFile(templateFilePath, outputFilePath, fields);
            return outputFilePath;
        }

        public string RenderPreview(string certificateUid, string templateUid, ImageFormat format, IDictionary<string, string> fields, bool resize = true)
        {
            Check.Argument.IsNotNull(fields, "fields");

            string generatedPdfPath = BuildPdfOutputPath(certificateUid, fields["ParticipantName"]);
            string imageFilePath = generatedPdfPath + FileExtensionFromEncoder(format);
            if (File.Exists(imageFilePath))
                return imageFilePath;

            var templateFilePath = GetPdfTempatePath(Guid.Parse(templateUid));
            RenderPdfFile(templateFilePath, generatedPdfPath, fields);

            using (var render = new GsNetPdfRender(generatedPdfPath, 150))
            {
                System.Drawing.Image emf = render.RenderPdfPage(1);
                if (resize)
                    emf = ResizeImage(emf, 640, 480);
                emf.Save(imageFilePath, format);
            }
            return imageFilePath;
        }

        //public void RenderPreview(Stream imageStream, ImageFormat format, IDictionary<string, string> fields)
        //{
        //    Check.Argument.IsNotNull(imageStream, "imageStream");
        //    Check.Argument.IsNotNull(fields, "fields");

        //    string generatedPdfPath = RenderPdfDocument(fields);
        //    using (var render = new GsNetPdfRender(generatedPdfPath, 150))
        //    {
        //        System.Drawing.Image emf = render.RenderPdfPage(1);
        //        emf = ResizeImage(emf, 640, 480);
        //        emf.Save(imageStream, format);
        //    }
        //}

        private string GetPdfTempatePath(Guid templateId)
        {
            string setting = _certificateSettings.PdfTemplateFolder;
            string folder = setting.StartsWith("~")
                ? Path.Combine(Directory.GetCurrentDirectory(), setting.TrimStart('~').TrimStart('/'))
                : setting;

            return Path.Combine(folder,
                string.Format("{0}.pdf", templateId.ToString()));
        }

        private void RenderPdfFile(string templateFilePath, string templateOutputPath, IDictionary<string, string> fields)
        {
            Check.Argument.IsNotNull(fields, "fields");

            if (File.Exists(templateOutputPath))
                return;

            using (var pdfOutputFile = new FileStream(templateOutputPath, FileMode.Create))
            {
                ProcessPdf(templateFilePath, pdfOutputFile, fields);
            }


            //PdfReader pdfReader = new PdfReader(certificateUid);
            //PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(certificateUid, FileMode.Create));

            //ProcessPdf(certificateUid, certificateUid, fields);
        }

        private void ProcessPdf(string templateFilePath, Stream outputStream, IDictionary<string, string> quizCertificateDetails)
        {
            using (var pdfReader = new PdfReader(templateFilePath))
            {
                using (var pdfStamper = new PdfStamper(pdfReader, outputStream))
                {
                    AcroFields testForm = pdfStamper.AcroFields;

                    foreach (var item in quizCertificateDetails)
                    {
                        if ((item.Key == "CourseName"))
                            testForm.SetFieldProperty(item.Key, "textsize", 0f, null);

                        testForm.SetField(item.Key, item.Value);
                    }
                }
            }
        }

        private string BuildPdfOutputPath(string certificateUid, string participantName)
        {
            string fileName = string.Format("{0}_{1}.pdf", certificateUid, participantName);

            string setting = _certificateSettings.PdfOutputFolder;
            string folder = setting.StartsWith("~") ?
                //? HostingEnvironment.MapPath(setting)
                Path.Combine(Directory.GetCurrentDirectory(), setting.TrimStart('~').TrimStart('/'))
                : setting;

            return Path.Combine(folder, fileName);
        }

        private static Bitmap ResizeImage(System.Drawing.Image srcBmp, int width, int height)
        {
            // Figure out the ratio
            double ratioX = (double)width / (double)srcBmp.Width;
            double ratioY = (double)height / (double)srcBmp.Height;
            // use whichever multiplier is smaller
            double ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(srcBmp.Height * ratio);
            int newWidth = Convert.ToInt32(srcBmp.Width * ratio);

            Bitmap target = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(target))
            {
                graphics.CompositingQuality = CompositingQuality.HighSpeed;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.DrawImage(srcBmp, 0, 0, newWidth, newHeight);
            }

            return target;
        }

        private static string FileExtensionFromEncoder(ImageFormat format)
        {
            try
            {
                return ImageCodecInfo.GetImageEncoders()
                        .First(x => x.FormatID == format.Guid)
                        .FilenameExtension
                        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .First()
                        .Trim('*')
                        .ToLower();
            }
            catch (Exception)
            {
                return ".JPG";
            }
        }

    }

}
