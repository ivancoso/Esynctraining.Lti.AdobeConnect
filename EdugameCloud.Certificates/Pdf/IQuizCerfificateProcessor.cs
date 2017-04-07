using System.Collections.Generic;
using System.Drawing.Imaging;

namespace EdugameCloud.Certificates.Pdf
{
    public interface IQuizCerfificateProcessor
    {
        string RenderPreview(string certificateUid, string templateUid, ImageFormat format, IDictionary<string, string> fields, bool resize = true);
        string RenderPdfDocument(string certificateUid, string templateUid, IDictionary<string, string> fields);
    }
}