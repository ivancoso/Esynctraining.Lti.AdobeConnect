namespace PDFAnnotation.Core.Utils
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using iTextSharp.text.pdf.parser;

    /// <summary>
    /// Custom test render strategy to extract character positions
    /// </summary>
    public class TextSymbolsExtractionStategy : ITextExtractionStrategy
    {
        //result buffer
        private readonly List<CharRenderInfo> result = new List<CharRenderInfo>();

        /// <summary>
        /// Extracts typed info from string result
        /// </summary>
        /// <param name="renderedResult">Render result string</param>
        /// <returns>list of <see cref="CharRenderInfo"/> items</returns>
        public static IEnumerable<CharRenderInfo> GetTypedResult(string renderedResult)
        {
            var serializer = new XmlSerializer(typeof(CharRenderInfo[]));
            using (var r = XmlReader.Create(new StringReader(renderedResult)))
            {
                return ((CharRenderInfo[])serializer.Deserialize(r)).AsEnumerable();
            }
        }

        /// <summary>
        /// Renders portion of text (single character or set of same-width characters).
        /// </summary>
        /// <param name="renderInfo">render information</param>
        public void RenderText(TextRenderInfo renderInfo)
        {
            var charInfo = new CharRenderInfo(renderInfo);
            if (charInfo.value.Length == 0)
            {
                return;
            }

            var charWidth = charInfo.width / charInfo.value.Length;
            var x = charInfo.x;
            var i = 0;
            foreach (var c in charInfo.value.ToCharArray())
            {
                var info = new CharRenderInfo
                {
                    x = x + charWidth * (i++),
                    y = charInfo.y,
                    width = charWidth,
                    height = charInfo.height,
                    fontSize = charInfo.fontSize,
                    value = c.ToString(System.Globalization.CultureInfo.InvariantCulture)
                };
                this.result.Add(info);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetResultantText()
        {
            int i = 0;

            this.result.ForEach(r => r.index = i++);

            var data = this.result.ToArray();
            var serializer = new XmlSerializer(typeof(CharRenderInfo[]));

            var sb = new StringBuilder();
            using (var w = XmlWriter.Create(sb))
            {
                serializer.Serialize(w, data);
            }

            return sb.ToString();
        }

        //Not needed
        public void BeginTextBlock() { }
        public void EndTextBlock() { }
        public void RenderImage(ImageRenderInfo renderInfo) { }
    }
}
