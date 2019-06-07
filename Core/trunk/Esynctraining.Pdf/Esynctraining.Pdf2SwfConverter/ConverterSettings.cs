namespace Esynctraining.Pdf2SwfConverter
{
    public class ConverterSettings
    {
        public string RenderAsBitmapArgs { get; set; } = "-s poly2bitmap";

        public string Pdf2SwfExecutableFilePath { get; set; }

        public string SwfToolsCommandPattern { get; set; } = @" ""{pdfFile}"" -o ""{swfFile}"" -f -T 9 -t -s storeallcharacters";
    }
}
