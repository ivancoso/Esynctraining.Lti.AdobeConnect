namespace PDFAnnotation.Core.Utils
{
    using System;
    using System.Reflection;

    using iTextSharp.text.pdf.parser;

    [Serializable]
    public class CharRenderInfo
    {
        public int index;
        public int pageIndex;
        public int x, y, width, height, fontSize;
        public string value;

        private static readonly FieldInfo GraphicStatePropInfo = typeof(TextRenderInfo).GetField("gs", BindingFlags.NonPublic | BindingFlags.Instance);

        public CharRenderInfo()
        {
        }

        public CharRenderInfo(TextRenderInfo renderInfo)
        {
            var curBaseline = renderInfo.GetBaseline().GetStartPoint();
            var topRight = renderInfo.GetAscentLine().GetEndPoint();

            this.value = renderInfo.GetText();

            this.x = (int)curBaseline[Vector.I1];
            this.y = (int)curBaseline[Vector.I2];

            this.width = (int)topRight[Vector.I1] - this.x;
            this.height = (int)topRight[Vector.I2] - this.y;

            var gs = (GraphicsState)GraphicStatePropInfo.GetValue(renderInfo);
            this.fontSize = (int)gs.FontSize;
        }
    }
}
