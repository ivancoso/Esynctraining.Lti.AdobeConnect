using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using Esynctraining.PdfProcessor.Renders;
using iTextSharp.text.pdf;

namespace EdugameCloud.Certificates.Pdf
{
    public sealed class QuizCerfificateProcessor
    {
        private readonly string _templateFilePath;
        private readonly string _templateToken;


        public QuizCerfificateProcessor(string templateFilePath, string templateToken)
        {
            Check.Argument.IsNotNullOrEmpty(templateFilePath, "templateFilePath");
            Check.Argument.IsNotNullOrEmpty(templateToken, "templateToken");

            _templateFilePath = templateFilePath;
            _templateToken = templateToken;
        }


        public string RenderPdfDocument(IDictionary<string, string> fields)
        {
            Check.Argument.IsNotNull(fields, "fields");

            string outputFilePath = BuildPdfPath(fields["ParticipantName"]);
            RenderPdfFile(outputFilePath, fields);
            return outputFilePath;
        }

        public string RenderPreview(ImageFormat format, IDictionary<string, string> fields, bool resize = true)
        {
            Check.Argument.IsNotNull(fields, "fields");

            string generatedPdfPath = BuildPdfPath(fields["ParticipantName"]);
            string imageFilePath = generatedPdfPath + FileExtensionFromEncoder(format);
            if (File.Exists(imageFilePath))
                return imageFilePath;

            RenderPdfFile(generatedPdfPath, fields);

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


        private void RenderPdfFile(string outputFilePath, IDictionary<string, string> fields)
        {
            Check.Argument.IsNotNull(fields, "fields");

            if (File.Exists(outputFilePath))
                return;

            using (var pdfOutputFile = new FileStream(outputFilePath, FileMode.Create))
            {
                ProcessPdf(pdfOutputFile, fields);
            }
        }

        private void ProcessPdf(Stream outputStream, IDictionary<string, string> quizCertificateDetails)
        {
            using (var pdfReader = new PdfReader(_templateFilePath))
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

        private string BuildPdfPath(string participantName)
        {
            string fileName = string.Format("{0}_{1}.pdf", _templateToken, participantName);

            string setting = ConfigurationManager.AppSettings["PdfOutputFolder"];
            string folder = setting.StartsWith("~")
                ? HostingEnvironment.MapPath(setting)
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
