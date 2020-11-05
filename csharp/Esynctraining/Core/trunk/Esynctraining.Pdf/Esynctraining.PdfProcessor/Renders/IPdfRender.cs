namespace Esynctraining.PdfProcessor.Renders
{
    using System;
    using System.Drawing;

    public interface IPdfRender : IDisposable
    {
        Image RenderPdfPage(int page);

    }

}