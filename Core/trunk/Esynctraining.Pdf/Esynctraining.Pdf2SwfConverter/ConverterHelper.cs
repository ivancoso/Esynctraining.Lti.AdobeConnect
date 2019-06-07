using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Esynctraining.Pdf.Common;
using Esynctraining.PdfProcessor;

namespace Esynctraining.Pdf2SwfConverter
{
    public class ConverterHelper
    {
        private const string Resources = "Esynctraining.Pdf2SwfConverter.Resources.";

        public static string SetupPdf2SwfNativeExeFromEmbeddedResources(string workingDir = null)
        {
            workingDir = string.IsNullOrWhiteSpace(workingDir) ? Directory.GetCurrentDirectory() : workingDir;

            var pdf2swfToolName = "pdf2swf.exe";
            
            string pdf2SwfToolPath = $"{workingDir}\\{pdf2swfToolName}";

            ResourceHelper.FlushResourceToFile(Assembly.GetAssembly(typeof(ConverterHelper)),
                Resources + pdf2swfToolName, pdf2SwfToolPath);

            return pdf2SwfToolPath;
        }
    }
}
