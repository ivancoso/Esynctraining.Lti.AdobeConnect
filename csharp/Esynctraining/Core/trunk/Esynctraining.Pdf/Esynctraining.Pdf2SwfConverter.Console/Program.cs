using System.IO;
using System.Threading.Tasks;
using Esynctraining.PdfProcessor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Esynctraining.Pdf2SwfConverter.Console
{
    class Program
    {
        static async Task Main()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory());
            builder.SetupAppSettingsAndLogging();
            var config = builder.Build();
            var logger = config.SetupSerilog();

            string inPdfPath = $"{Directory.GetCurrentDirectory()}\\BlackRectanglesWithSomeText.pdf";
            
            var workingDir = Directory.CreateDirectory("output");

            var converterSettings = new ConverterSettings
            {
                Pdf2SwfExecutableFilePath = ConverterHelper.SetupPdf2SwfNativeExeFromEmbeddedResources()
            };

            var gs = PdfProcessorHelper.SetupGhostScriptFromEmbeddedResources();

            var pdfProcesserSettings = new PdfProcessorSettings
            {
                SearchForGhosts = false, 
                GhostScriptDllPath = gs.GhostScriptDllPath,
                GhostScriptVersion = gs.GhostScriptVersion
            };

            var converter = new Converter(converterSettings, new PdfProcessorHelper(pdfProcesserSettings), logger);

            var status = await converter.ConvertPdf2Swf(inPdfPath, new ConversionOptions
            {
                HowToConvert = ConversionPath.Both,
                OutputDirectory = workingDir.FullName,
                OverwriteExistingSwfs = true
            });

            logger.LogWarning($"Result is {status.State}");

            System.Console.ReadKey();

        }
    }
}
