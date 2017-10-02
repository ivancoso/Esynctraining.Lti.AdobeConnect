using System.Text.RegularExpressions;
using PDFAnnotation.Core.Wrappers;


namespace PDFAnnotation.Core.Business.Models.Annotation
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using com.wiris.editor.services;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Providers;

    using iTextSharp.text;
    using iTextSharp.text.pdf;
    using iTextSharp.text.pdf.parser;

    using PDFAnnotation.Core.Constants;
    using PDFAnnotation.Core.Domain.DTO;
    using PDFAnnotation.Core.Domain.Entities;
    using PDFAnnotation.Core.Utils;

    using WebException = WcfRestContrib.ServiceModel.Web.Exceptions.WebException;

    /// <summary>
    /// The PDF model.
    /// </summary>
    public class PdfModel
    {
        /// <summary>
        /// The converter.
        /// </summary>
        private readonly Pdf2SwfConverter converter;

        /// <summary>
        /// The settings.
        /// </summary>
        private readonly dynamic settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfModel"/> class.
        /// </summary>
        /// <param name="converter">
        /// The converter.
        /// </param>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public PdfModel(Pdf2SwfConverter converter, ApplicationSettingsProvider settings)
        {
            PdfReader.unethicalreading = true;
            this.converter = converter;
            this.settings = settings;
        }

        /// <summary>
        /// The convert if not exist.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <param name="ms">
        /// The memory stream.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConvertIfNotExist(Domain.Entities.File file, byte[] ms = null)
        {
            var failedFolder = (string)System.IO.Path.Combine(FileModel.FileStoragePhysicalPath(this.settings), (string)this.settings.FailedPDFsFolder);
            var connectionString = (string)this.settings.ConnectionString;
            return this.converter.ConvertIfNotExist(new FileDTO(file), failedFolder, connectionString, ms);
        }

        /// <summary>
        /// The get number of pages.
        /// </summary>
        /// <param name="file">
        /// The web orb file.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int32}"/>.
        /// </returns>
        public int? GetNumberOfPages(string file)
        {
            try
            {
                using (var reader = new PdfReader(file))
                {
                    return reader.NumberOfPages;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// The get number of pages using stream.
        /// </summary>
        /// <param name="file">
        /// The file.
        /// </param>
        /// <returns>
        /// The <see cref="Nullable{Int32}"/>.
        /// </returns>
        public int? GetNumberOfPagesUsingStream(string file)
        {
            using (var sr = new StreamReader(System.IO.File.OpenRead(file)))
            {
                var regex = new Regex(@"/Type\s*/Page[^s]");
                var matches = regex.Matches(sr.ReadToEnd());
                return matches.Count;
            }
        }

        /// <summary>
        /// The convert if not exist.
        /// </summary>
        /// <param name="fileDTO">
        /// The file DTO.
        /// </param>
        /// <param name="ms">
        /// The memory stream.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ConvertIfNotExist(FileDTO fileDTO, byte[] ms = null)
        {
            var failedFolder = (string)System.IO.Path.Combine(FileModel.FileStoragePhysicalPath(this.settings), (string)this.settings.FailedPDFsFolder);
            var connectionString = (string)this.settings.ConnectionString;
            return this.converter.ConvertIfNotExist(fileDTO, failedFolder, connectionString, ms);
        }

        /// <summary>
        /// The draw on pdf.
        /// </summary>
        /// <param name="drawings">
        /// The drawings.
        /// </param>
        /// <param name="highlights">
        /// The highlights.
        /// </param>
        /// <param name="shapes">
        /// The shapes.
        /// </param>
        /// <param name="textItems">
        /// The text items.
        /// </param>
        /// <param name="pictures">
        /// The text items.
        /// </param>
        /// <param name="rotations">
        /// The rotations.
        /// </param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <returns>
        /// The <see cref="byte"/>.
        /// </returns>
        public byte[] DrawOnPDF(IEnumerable<ATMark> marks,byte[] buffer)
        {
            return this.DrawOnPDFOrdered( marks, buffer);
        }

        /// <summary>
        /// The draw on PDF.
        /// </summary>
        /// <param name="drawings">
        /// The drawings.
        /// </param>
        /// <param name="textItems">the text items</param>
        /// <param name="rotations">final page rotation angles</param>
        /// <param name="buffer">
        /// The buffer.
        /// </param>
        /// <param name="highlights">the highlights</param>
        /// <param name="shapes">the shapes</param>
        /// <param name="pictures">the pictures</param>
        /// <returns>
        /// The byte array/>.
        /// </returns>
        /// <exception cref="WcfRestContrib.ServiceModel.Web.Exceptions.WebException">
        /// When something went wrong
        /// </exception>
        public byte[] DrawOnPDFOrdered(IEnumerable<ATMark> marks,byte[] buffer)
        {
            try
            {
                //var pageRotations = marks.SelectMany(x => x.Rotations).ToList();
                var shapesList = marks.SelectMany(x => x.Shapes).ToList();

                var pageRules = shapesList.FirstOrDefault(x => x.Mark != null && x.Mark.Type == EntityTypes.PageRules);

                if (pageRules != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        var reader = new PdfReader(buffer);
                        using (var stamper = new PdfStamper(reader, ms))
                        {
                            this.ProcessRules(pageRules, stamper, reader);
                            shapesList.Remove(pageRules);
                            stamper.Close();
                        }
                        ms.Flush();
                        buffer = ms.ToArray();
                    }
                }
                foreach (var mk in marks)
                {
                    using (var ms = new MemoryStream())
                    {
                        var reader = new PdfReader(buffer);
                        using (var stamper = new PdfStamper(reader, ms))
                        {
                            foreach (var drawing in mk.Drawings)
                            {
                                this.ProcessDrawings(drawing, stamper, reader);
                            }
                            foreach (var highlight in mk.HighlightStrikeOuts)
                            {
                                this.ProcessHighlights(highlight, stamper, reader);
                            }
                            foreach (var shape in mk.Shapes)
                            {
                                this.ProcessShapes(shape, stamper, reader);
                            }
                            foreach (var text in mk.TextItems)
                            {
                                this.ProcessText(text, stamper, reader);
                            }
                            foreach (var picture in mk.Pictures)
                            {
                                this.ProcessPicture(picture, stamper, reader);
                            }
                            foreach (var formula in mk.Formulas)
                            {
                                this.ProcessFormula(formula, stamper, reader);
                            }
                            //foreach (var rotation in mk.Rotations)
                            //{
                            //    this.ProcessRotation(rotation, stamper, reader);
                            //}
                            foreach (var annotation in mk.Annotations)
                            {
                                this.ProcessAnnotation(annotation, stamper, reader);
                            }
                            if (mk.Type == EntityTypes.Rotation)
                            {
                                this.ProcessRotation(mk, stamper, reader);
                            }

                            stamper.Close();
                        }
                        ms.Flush();
                        buffer = ms.ToArray();
                    }
                }

                return buffer;
            }
            catch (Exception ex)
            {
                throw new WebException(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }


        /// <summary>
        /// Gets all symbol positions and values from file buffer
        /// </summary>
        /// <param name="buffer">pdf file data</param>
        /// <param name="pageIndex">page index</param>
        /// <returns>collection of text symbols in document</returns>
        public IEnumerable<CharRenderInfo> GetAllSymbols(byte[] buffer, int? pageIndex = null)
        {
            try
            {
                var reader = new PdfReader(buffer);
                return this.GetAllSymbols(reader, pageIndex);
            }
            catch (Exception ex)
            {
                throw new WebException(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        /// <summary>
        /// Gets all symbol positions and values from pdf reader
        /// </summary>
        /// <param name="reader">pdf file reader</param>
        /// <param name="pageIndex">page index</param>
        /// <returns>collection of text symbols in document</returns>
        public IEnumerable<CharRenderInfo> GetAllSymbols(PdfReader reader, int? pageIndex = null)
        {
            try
            {
                var result = new List<CharRenderInfo>();

                var indexes = pageIndex.HasValue
                                  ? new List<int> { pageIndex.Value }
                                  : Enumerable.Range(1, reader.NumberOfPages).ToList();
                foreach (var index in indexes)
                {
                    var pageHeight = reader.GetPageSizeWithRotation(index).Height;
                    var resultString = PdfTextExtractor.GetTextFromPage(reader, index, new TextSymbolsExtractionStategy());
                    var pageData = TextSymbolsExtractionStategy.GetTypedResult(resultString);
                    var page = index;
                    pageData = pageData.Select(ci =>
                    {
                        ci.pageIndex = page;
                        ci.y = (int) (pageHeight - ci.y);
                        return ci;
                    });
                    result.AddRange(pageData);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw new WebException(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        /// <summary>
        /// Creates new PDF document with requested number of empty pages
        /// </summary>
        /// <param name="pageCount">page count (single page by default)</param>
        /// <returns>pdf document content</returns>
        public byte[] CreateNew(int pageCount = 1)
        {
            byte[] result;
            using (var ms = new MemoryStream())
            {
                var doc = new Document(PageSize.A4, 0, 0, 0, 0);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();
                FontFactory.GetFont("Arial");

                for (int i = 0; i < pageCount; i++)
                {
                    doc.NewPage();
                }

                doc.Close();
                result = ms.GetBuffer();
            }

            return result;
        }

        /// <summary>
        /// The process shapes.
        /// </summary>
        /// <param name="shapes">
        /// The shapes.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="pdfReader">
        /// The pdf reader.
        /// </param>
        private void ProcessShapes(ATShape dr /* IEnumerable<ATShape> shapes*/, PdfStamper stamper, PdfReader pdfReader)
        {
            //foreach (var dr in shapes)
           // {
                switch (dr.Mark.Type)
                {
                    case EntityTypes.PageRules:
                        //continue;
                        break;
                    case EntityTypes.Confidential:
                    case EntityTypes.ConfidentialAttorney:
                    case EntityTypes.HighlyConfidential:
                        this.ProcessConfidentialStamp(dr, stamper, pdfReader);
                        break;
                    case EntityTypes.Exhibit:
                    case EntityTypes.ExhibitDefendant:
                    case EntityTypes.ExhibitPlaintiff:
                        this.ProcessExhibitStamp(dr, stamper, pdfReader);
                        break;
                    case EntityTypes.Stamp:
                        switch ((dr.Style ?? string.Empty).ToLowerInvariant())
                        {
                            case "solid":
                                this.ProcessStampSolidOutline(dr, stamper, pdfReader, true);
                                break;
                            case "panelstamp":
                                this.ProcessExhibitStamp(dr, stamper, pdfReader);
                                break;
                            case "outline":
                                this.ProcessStampSolidOutline(dr, stamper, pdfReader, false);
                                break;
                            default:
                                //continue;
                                break;
                        }
                        break;
                    case EntityTypes.Ellipse:
                    case EntityTypes.FilledEllipse:
                        this.ProcessEllipse(dr, stamper, pdfReader);
                        break;
                    case EntityTypes.Rectangle:
                    case EntityTypes.FilledRectangle:
                        this.ProcessRectangle(dr, stamper, pdfReader);
                        break;
                    case EntityTypes.Drawing:
                        this.ProcessDraw(dr, stamper, pdfReader);
                        break;
                    case EntityTypes.HighlightLine:
                        this.ProcessDraw(dr,stamper,pdfReader);
                        break;
                    default:
                        this.ProcessShape(dr, stamper, pdfReader);
                        break;
                }
            //}
        }

        private void ProcessExhibitStamp(ATShape dr, PdfStamper stamper, PdfReader pdfReader)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;
            var cb = stamper.GetOverContent(dr.Mark.PageIndex);

            var outer = new Rectangle(dr.PositionX, pageHeight - (dr.PositionY + dr.Height), dr.PositionX + dr.Width, pageHeight - dr.PositionY);
            var color = GetColor(dr.StampColor ?? "#2036E0");
            cb.SetColorStroke(color);
            cb.SetColorFill(color);
            cb.SetLineWidth(0);
            cb.RoundRectangle(outer.Left + 2, outer.Bottom, outer.Width - 2, outer.Height, 5);
            cb.FillStroke();

            const float shift = 8;
            var inner = new Rectangle(outer.Left + shift, outer.Bottom + shift / 2, outer.Right - shift, outer.Bottom + shift / 2 + 30);
            var whiteFill = GetColor("#FFFFFF");
            cb.SetColorStroke(whiteFill);
            cb.SetColorFill(whiteFill);
            cb.RoundRectangle(inner.Left + 2, inner.Bottom, inner.Width - 1, inner.Height, 4);
            cb.FillStroke();

            // adjust outer for text alignment
            outer.Bottom = inner.Top;

            var text = dr.LabelText ??
                       (dr.Mark.Type == EntityTypes.ExhibitDefendant
                           ? "Defendant's\nExhibit"
                           : dr.Mark.Type == EntityTypes.ExhibitPlaintiff
                               ? "Plaintiff'sExhibit"
                               : "Exhibit");

            cb.BeginText(); // Start working with text.

            // Create a font to work with 
            var labelTextColor = !string.IsNullOrEmpty(dr.LabelTextColor) ? GetColor(dr.LabelTextColor) : whiteFill;
            cb.SetRGBColorFill(labelTextColor.R, labelTextColor.G, labelTextColor.B);

            var size = 12f;
            var baseFont = BaseFont.CreateFont(BaseFont.TIMES_BOLD, Encoding.ASCII.EncodingName, true);
            var ct = new ColumnText(cb);
            var p = new Paragraph(text, new Font(baseFont, size));
            var t = new PdfPTable(1);
            var c = new PdfPCell(p)
            {
                VerticalAlignment = Element.ALIGN_MIDDLE,
                HorizontalAlignment = Element.ALIGN_CENTER,
            };
            c.FixedHeight = outer.Height + 10;
            c.BorderWidth = 0;
            c.SetLeading(0f, 1.2f);
            c.PaddingBottom = c.PaddingLeft = c.PaddingRight = c.PaddingTop = 0;
            t.AddCell(c);
            ct.SetSimpleColumn(outer.Left, outer.Bottom, outer.Right, outer.Top + 10);
            ct.AddElement(t);
            ct.Go();

            cb.EndText(); // Done working with text

            if (!string.IsNullOrWhiteSpace(dr.Text))
            {
                cb.BeginText(); // Start working with text.

                // Create a font to work with 
                baseFont = BaseFont.CreateFont(BaseFont.HELVETICA_BOLD, Encoding.ASCII.EncodingName, false);
                cb.SetFontAndSize(baseFont, 14); // 40 point font
                var textColor =
                    GetColor(!string.IsNullOrEmpty(dr.NumberingTextColor) ? dr.NumberingTextColor : "#000000");
                cb.SetRGBColorFill(textColor.R, textColor.G, textColor.B); 

                // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                // This command tells iTextSharp to write the text at a certain location with a certain angle.
                // Again, this will angle the text from bottom left corner to top right corner and it will 
                // place the text in the middle of the page. 
                cb.ShowTextAligned(
                    PdfContentByte.ALIGN_CENTER,
                    dr.Text,
                    dr.PositionX + dr.Width/2,
                    pageHeight - (dr.PositionY + dr.Height/2 + 18),
                    0);

                cb.EndText(); // Done working with text
            }
        }

        private void ProcessConfidentialStamp(ATShape dr, PdfStamper stamper, PdfReader pdfReader)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;
            var cb = stamper.GetOverContent(dr.Mark.PageIndex);

            var outer = new Rectangle(dr.PositionX, pageHeight - (dr.PositionY + dr.Height), dr.PositionX + dr.Width, pageHeight - dr.PositionY);
            var color = GetColor("#990000");
            cb.SetColorStroke(color);
            var strokeWidth = 4f;
            cb.SetLineWidth(strokeWidth);
            cb.RoundRectangle(
                outer.Left,
                outer.Bottom + strokeWidth,
                outer.Width,
                outer.Height - strokeWidth,
                5);
            cb.Stroke();

            var text = dr.Mark.Type == EntityTypes.ConfidentialAttorney
                ? "CONFIDENTIAL\nATTORNEY'S\nEYES ONLY"
                : dr.Mark.Type == EntityTypes.HighlyConfidential
                    ? "HIGHLY\nCONFIDENTIAL"
                    : "CONFIDENTIAL";

            // Create a font to work with 
            cb.SetRGBColorFill(color.R, color.G, color.B);

            // Note: The x,y of the Pdf Matrix is from bottom left corner. 
            // This command tells iTextSharp to write the text at a certain location with a certain angle.
            // Again, this will angle the text from bottom left corner to top right corner and it will 
            // place the text in the middle of the page.
            var size = 13f;
            var baseFont = BaseFont.CreateFont(BaseFont.TIMES_BOLD, Encoding.ASCII.EncodingName, true);
            var ct = new ColumnText(cb);
            var p = new Paragraph(text, new Font(baseFont, size));
            var t = new PdfPTable(1);
            var c = new PdfPCell(p);
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = outer.Height + 10;
            c.BorderWidth = 0;
            c.SetLeading(0f, 1.2f);
            c.PaddingBottom = c.PaddingLeft = c.PaddingRight = c.PaddingTop = 0;
            t.AddCell(c);
            // TODO: remove this hardcoded adjust, was added to conform client
            ct.SetSimpleColumn(outer.Left - 2, outer.Bottom, outer.Right + 2, outer.Top + 12); 
            ct.AddElement(t);
            ct.Go();
        }

        private void ProcessStampSolidOutline(ATShape dr, PdfStamper stamper, PdfReader pdfReader, bool isFilled)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;
            var cb = stamper.GetOverContent(dr.Mark.PageIndex);

            var outer = new Rectangle(dr.PositionX, pageHeight - (dr.PositionY + dr.Height), dr.PositionX + dr.Width, pageHeight - dr.PositionY);
            var color = GetColor(dr.StampColor ?? "#990000");
            cb.SetColorStroke(color);
            var strokeWidth = 4f;
            cb.SetLineWidth(strokeWidth);
            cb.RoundRectangle(
                outer.Left,
                outer.Bottom + strokeWidth,
                outer.Width - strokeWidth,
                outer.Height - strokeWidth,
                5);

            if (isFilled)
            {
                cb.SetColorFill(color);
                cb.FillStroke();
            }
            else
            {
                cb.Stroke();
            }

            var text = dr.LabelText;

            // Create a font to work with 
            color = GetColor(dr.LabelTextColor ?? "#000000");
            cb.SetRGBColorFill(color.R, color.G, color.B);

            // Note: The x,y of the Pdf Matrix is from bottom left corner. 
            // This command tells iTextSharp to write the text at a certain location with a certain angle.
            // Again, this will angle the text from bottom left corner to top right corner and it will 
            // place the text in the middle of the page.
            var size = 13f;
            var baseFont = BaseFont.CreateFont(BaseFont.TIMES_BOLD, Encoding.ASCII.EncodingName, true);
            var ct = new ColumnText(cb);
            var p = new Paragraph(text, new Font(baseFont, size));
            var t = new PdfPTable(1);
            var c = new PdfPCell(p);
            c.VerticalAlignment = Element.ALIGN_MIDDLE;
            c.HorizontalAlignment = Element.ALIGN_CENTER;
            c.FixedHeight = outer.Height + 10;
            c.BorderWidth = 0;
            c.SetLeading(0f, 1.2f);
            c.PaddingBottom = c.PaddingLeft = c.PaddingRight = c.PaddingTop = 0;
            t.AddCell(c);
            // TODO: remove this hardcoded adjust, was added to conform client
            ct.SetSimpleColumn(outer.Left - 2, outer.Bottom, outer.Right + 2, outer.Top + 12);
            ct.AddElement(t);
            ct.Go();
        }

        private void ProcessText(ATTextItem dr, PdfStamper stamper, PdfReader pdfReader)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;
            var cb = stamper.GetOverContent(dr.Mark.PageIndex);

            var bc = GetColor(dr.Color);
            cb.SetColorStroke(bc);
            cb.SetRGBColorFill(bc.R, bc.G, bc.B);

            cb.BeginText(); // Start working with text.

            // Create a font to work with 
            var fontName = !string.IsNullOrEmpty(dr.FontName) ? dr.FontName : "Arial";
            var size = dr.FontSize > 0 ? dr.FontSize : 14;
            var font = FontFactory.GetFont(fontName, size, Font.NORMAL);
            var baseFont = font.GetCalculatedBaseFont(false);
            cb.SetFontAndSize(baseFont, size);

            // Note: The x,y of the Pdf Matrix is from bottom left corner. 
            // This command tells iTextSharp to write the text at a certain location with a certain angle.
            // Again, this will angle the text from bottom left corner to top right corner and it will 
            // place the text in the middle of the page. 
            var y = pageHeight - ((float) dr.PositionY + size);
            foreach (var part in dr.Text.Split('\n'))
            {
                cb.ShowTextAligned(
                    PdfContentByte.ALIGN_LEFT,
                    part,
                    (float) dr.PositionX + 3, 
                    y,
                    0);
                y -= (size + 4);
            }

            cb.EndText(); // Done working with text
        }

        /// <summary>
        /// The process highlights.
        /// </summary>
        /// <param name="highlights">
        /// The highlights.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="pdfReader">The reader</param>
        private void ProcessHighlights(ATHighlightStrikeOut dr /* IEnumerable<ATHighlightStrikeOut> highlights*/, PdfStamper stamper, PdfReader pdfReader)
        {
            //foreach (var dr in highlights.Where(h => h.HasSelection))
            {
                var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;

                var selectionInfo = dr.SelectionInfo.Split(new[] {';', ','});
                var selectionStart = int.Parse(selectionInfo[1]);
                var selectionEnd = int.Parse(selectionInfo[2]);

                var symbols = this.GetAllSymbols(pdfReader).Skip(selectionStart).Take(selectionEnd - selectionStart + 1).ToList();
                //Create a inner rectangles for the highlight. NOTE: Technically this isn't used but it helps with the quadpoint calculation
                var rectangles = this.GetInnerHighlights(symbols).ToList();
                var overlapped = new List<Rectangle>();
                for (var i = 0; i < rectangles.Count; i++)
                {
                    var rect = rectangles[i];
                    for (var j = i + 1; j < rectangles.Count; j++, i++)
                    {
                        bool sameSize = Math.Abs(rectangles[j].Height - rect.Height) < float.Epsilon &&
                                        Math.Abs(rectangles[j].Top - rect.Top) < float.Epsilon;
                        if (!sameSize)
                        {
                            break;
                        }

                        rect = new Rectangle(rect.Left, rect.Bottom,
                            rect.Right + rectangles[j].Width, rect.Top);
                    }

                    overlapped.Add(rect);
                }

                foreach (var rect in overlapped)
                {
                    //Add the annotation
                    var over = stamper.GetOverContent(dr.Mark.PageIndex);
                    over.SaveState();
                    over.SetGState(new PdfGState {FillOpacity = 0.3f, StrokeOpacity = 0.3f});

                    //Set the color
                    var bc = GetColor(dr.Color);
                    over.SetColorStroke(bc);
                    over.SetRGBColorFill(bc.R, bc.G, bc.B);
                    over.SetLineWidth(2);
                    over.Rectangle(rect.Left, pageHeight - rect.Top, rect.Width, rect.Height);
                    over.FillStroke();

                    over.RestoreState();
                }
            }
        }


        /// <summary>
        /// The process pictures.
        /// </summary>
        /// <param name="picture">The pictures</param>
        /// <param name="stamper">The stamper</param>
        /// <param name="pdfReader">The pdfReader</param>
        private void ProcessPicture(ATPicture picture /*IEnumerable<ATPicture> pictures*/, PdfStamper stamper, PdfReader pdfReader)
        {
            //foreach (var pc in pictures)
            {
               PdfContentByte cb = stamper.GetOverContent(picture.Mark.PageIndex);
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(picture.Image);
                var size = pdfReader.GetPageSizeWithRotation(picture.Mark.PageIndex);
                // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                image.SetAbsolutePosition(picture.PositionX, size.Height - picture.PositionY - image.Height);
                //Note: The PdfGState is required to set the transparency for the layer
                PdfGState state = new PdfGState();
                state.FillOpacity = 1.0f;
                cb.SetGState(state);
                cb.AddImage(image);
                // stamper.Close();

                var bc = GetColor(picture.LabelTextColor);
                cb.SetColorStroke(bc);
                cb.SetRGBColorFill(bc.R, bc.G, bc.B);

                cb.BeginText(); // Start working with text.

                // Create a font to work with 
                var fontName =  "Arial";
                var fontSize = picture.LabelFontSize > 0 ? picture.LabelFontSize : 14;
                var font = FontFactory.GetFont(fontName, fontSize, Font.NORMAL);
                var baseFont = font.GetCalculatedBaseFont(false);
                cb.SetFontAndSize(baseFont, fontSize);

                // Note: The x,y of the Pdf Matrix is from bottom left corner. 
                // This command tells iTextSharp to write the text at a certain location with a certain angle.
                // Again, this will angle the text from bottom left corner to top right corner and it will 
                // place the text in the middle of the page. 
                var y = size.Height - ((float)picture.PositionY +image.Height + fontSize);
                foreach (var part in picture.LabelText.Split('\n'))
                {
                    cb.ShowTextAligned(
                        PdfContentByte.ALIGN_CENTER,
                        part,
                        (float)picture.PositionX +image.Width/2,
                        y,
                        0);
                    y -= (fontSize + 4);
                }

                cb.EndText(); // Done working with text
            }
        }


        /// <summary>
        /// The process rotation
        /// </summary>
        /// <param name="rotation">The rotation</param>
        /// <param name="stamper">The stamper</param>
        /// <param name="pdfReader">The pdfReader</param>
        private void ProcessRotation(ATMark mk, /*ATRotation rt,*/ PdfStamper stamper, PdfReader pdfReader)
        {
            var rotationOrigin = pdfReader.GetPageRotation(mk.PageIndex);
            var pageDict = pdfReader.GetPageN(mk.PageIndex);
            int rotation = mk.Rotation.Return(x => (int)Math.Round(x.Value), 0);
            pageDict.Put(PdfName.ROTATE, new PdfNumber(rotationOrigin + 90));
        }


        /// <summary>
        /// The process formula.
        /// </summary>
        /// <param name="formula">The formula</param>
        /// <param name="stamper">The stamper</param>
        /// <param name="pdfReader">The pdfReader</param>
        private void ProcessFormula(ATFormula formula, PdfStamper stamper, PdfReader pdfReader)
        {
            PdfContentByte cb = stamper.GetOverContent(formula.Mark.PageIndex);

            Dictionary<string, string> request = new Dictionary<string, string>();
            request["format"] = "png";
            request["mml"] = formula.Equation;
            Dictionary<string, string> response = new Dictionary<string, string>();
            byte[] imageBytes = PublicServices.getInstance().renderPng(formula.Equation, null, request, response);
            //var svg = PublicServices.getInstance().renderSvg(formula.Equation,null, null, response);
            int width = int.Parse(response["width"]);
            int height = int.Parse(response["height"]);


            //byte[] imageBytes;

            //using (var client = new WebClient())
            //{
            //    var values = new NameValueCollection();
            //    values["format"] = "png";
            //    values["mml"] = formula.Equation;

            //    var response = client.UploadValues((string)this.settings.WirisEditorServiceURL, values);
            //    imageBytes = response;
            //}

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(imageBytes);
            var size = pdfReader.GetPageSizeWithRotation(formula.Mark.PageIndex);
            // Note: The x,y of the Pdf Matrix is from bottom left corner. 
            image.SetAbsolutePosition(formula.PositionX, size.Height - formula.PositionY - image.Height);
            cb.AddImage(image);

        }


        /// <summary>
        /// The process annotation.
        /// </summary>
        /// <param name="annotation">The formula</param>
        /// <param name="stamper">The stamper</param>
        /// <param name="pdfReader">The pdfReader</param>
        private void ProcessAnnotation(ATAnnotation annotation, PdfStamper stamper, PdfReader pdfReader)
        {
            var size = pdfReader.GetPageSizeWithRotation(annotation.Mark.PageIndex);
            var defaultColor = "#FFF88A";
            //var color = PdfModel.GetColor(string.IsNullOrWhiteSpace(annotation.Color)?defaultColor:annotation.Color); // Temporary comment out
            var border = new Rectangle(annotation.PositionX, size.Height - (annotation.PositionY + annotation.Height),
                    annotation.PositionX + annotation.Width, size.Height - annotation.PositionY);

            
    
            border.Normalize();
            if (Math.Abs(border.Width) > 0 && Math.Abs(border.Height) > 0) {
                var markupAnnot = PdfAnnotation.CreateText(
                    stamper.Writer,
                    border,
                    annotation.CreatedBy,  // string.Empty, // text for title
                    annotation.Comment,
                    annotation.IsOpened,
                        //StickyNoteIconsNames.GetName(annotation.IconName)
                    "Comment"
                    );
                    //if (annotation.FillOpacity.HasValue)
                    //{
                    //    markupAnnot.Put(PdfName.CA, new PdfNumber(annotation.FillOpacity.Value));
                    //}

               markupAnnot.Put(PdfName.CA, new PdfNumber(100));
               markupAnnot.Color = PdfModel.GetColor(defaultColor); //color;
                 //   markupAnnot.Title = annotation.CreatedBy;      //string.Empty; // text for title
               markupAnnot.Put(PdfName.CREATIONDATE, new CustomPDFDate(annotation.Mark.DateCreated));
               markupAnnot.Put(PdfName.M, new CustomPDFDate(annotation.Mark.DateChanged));

                  //  PdfGState state = new PdfGState();
                 //   state.FillOpacity = 1.0f;
                 //   cb.SetGState(state);

               stamper.AddAnnotation(markupAnnot, annotation.Mark.PageIndex);
            }
        }


        


        /// <summary>
        /// Splits continuous highlight by rectangles bound to single line parts.
        /// </summary>
        /// <param name="symbols">selected symbols</param>
        /// <returns>list of line-bound rectangles</returns>
        private IEnumerable<Rectangle> GetInnerHighlights(IList<CharRenderInfo> symbols)
        {
            var rectangleParts = new List<Rectangle>();
            var current = symbols.First();
            var currentRect = new Rectangle(current.x, current.y, current.x, current.y - current.height);
            foreach (var symbol in symbols)
            {
                if (symbol.y == (int) currentRect.Bottom && symbol.height == (int) currentRect.Height)
                {
                    currentRect = new Rectangle(
                        currentRect.Left,
                        currentRect.Bottom,
                        currentRect.Right + symbol.width,
                        currentRect.Bottom - Math.Max(currentRect.Height, symbol.height));
                    continue;
                }

                rectangleParts.Add(currentRect);
                currentRect = new Rectangle(symbol.x, symbol.y, symbol.x + symbol.width, symbol.y - symbol.height);
            }

            if ((int) currentRect.Width > 0)
            {
                rectangleParts.Add(currentRect);
            }

            return rectangleParts.Select(r =>
            {
                var h = Math.Abs(r.Height);
                return new Rectangle(r.Left, r.Bottom + h*(float) .2, r.Right, r.Bottom - h*(float) 1.2);
            }).ToList();
        }

        /// <summary>
        /// The process drawings.
        /// </summary>
        /// <param name="drawings">
        /// The drawings.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        private void ProcessDrawings(ATDrawing dr /*IEnumerable<ATDrawing> drawings*/, PdfStamper stamper, PdfReader reader)
        {
           // foreach (var dr in drawings)
           // {
                PdfContentByte cb = stamper.GetOverContent(dr.Mark.PageIndex);
                var size = reader.GetPageSizeWithRotation(dr.Mark.PageIndex);
                var bc = GetColor(dr.Color);
                cb.SetColorStroke(bc);
                cb.SetLineWidth(2);
                var parsedPoints = this.ParseDrawing(dr.Points, size).ToList();
                if (parsedPoints.Any())
                {
                    var firstPoint = parsedPoints.First();
                    cb.MoveTo(firstPoint.X, firstPoint.Y);
                    foreach (var point in parsedPoints)
                    {
                        cb.LineTo(point.X, point.Y);
                    }
                    cb.Stroke();
                }
           // }
        }

        /// <summary>
        /// The process drawings.
        /// </summary>
        /// <param name="shape">
        /// The drawings.
        /// </param>
        /// <param name="drawAction">actual draw action</param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        private void ProcessShape(ATShape shape, Action<PdfContentByte, ATShape> drawAction, PdfStamper stamper)
        {
            var cb = stamper.GetOverContent(shape.Mark.PageIndex);

            var bc = GetColor(shape.Color);
            cb.SetColorStroke(bc);

            var isFilled = shape.Mark.Type == EntityTypes.FilledEllipse ||
                           shape.Mark.Type == EntityTypes.FilledRectangle;

            if (isFilled)
            {
                cb.SetColorFill(bc);
            }

            cb.SetLineWidth(shape.StrokeWidth);
            cb.SetLineCap(PdfContentByte.LINE_CAP_PROJECTING_SQUARE);
            cb.SetLineJoin(PdfContentByte.LINE_JOIN_MITER);

            cb.SaveState();
            var gs = new PdfGState();
            var opacity = Math.Max(0, Math.Max(shape.FillOpacity, shape.StrokeOpacity));
            if (opacity > 0)
            {
                gs.FillOpacity = opacity;
                gs.StrokeOpacity = opacity;
            }

            cb.SetGState(gs);

            drawAction(cb, shape);

            if (isFilled)
            {
                cb.FillStroke();
            }
            else
            {
                cb.Stroke();
            }

            cb.RestoreState();
        }

        public byte[] AddNewPage(byte[] buffer, byte[] redredContent)
        {
            using (var ms = new MemoryStream())
            {
                var reader = new PdfReader(buffer);
                var readerRenderedContent = new PdfReader(redredContent);
                PdfWriter writer = null;
                Document document = new Document();
                try
                {
                    //Step 2: we create a writer that listens to the document
                    writer = PdfWriter.GetInstance(document, ms);


                    //Step 3: Open the document
                    document.Open();

                    PdfContentByte cb = writer.DirectContent;

                    for (int pageNumber = 1; pageNumber < reader.NumberOfPages + 1; pageNumber++)
                    {
                        document.SetPageSize(reader.GetPageSizeWithRotation(1));
                        document.NewPage();

                        //Insert to Destination on the first page
                        if (pageNumber == 1)
                        {
                            Chunk fileRef = new Chunk(" ");
                            document.Add(fileRef);
                        }

                        PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                        int rotation = reader.GetPageRotation(pageNumber);                    

  
                        if (rotation == 90 || rotation == 270)
                        {
                            cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                        }
                        else
                        {
                            cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                        }
                    }

                    document.SetPageSize(readerRenderedContent.GetPageSizeWithRotation(readerRenderedContent.NumberOfPages));
                    document.NewPage();

                    Paragraph paragraph = new Paragraph();
                    Chunk titleChunk = new Chunk(" ");
                    paragraph.Add(titleChunk);
                    document.Add(paragraph);
                }
                finally
                {
                    document.Close();
                }

                ms.Flush();
                buffer = ms.ToArray();

                if (reader != null)
                {
                    reader.Close();
                }

                if (writer != null)
                {
                    writer.Close();
                }
            }

            return buffer;
        }



        public byte[] DeletePage(byte[] buffer, int[] pageIndexes)
        {
            using (var ms = new MemoryStream()) //
            {
                var reader = new PdfReader(buffer);//
                PdfWriter writer = null;//
                Document document = new Document();//
                try
                {
                    //Step 2: we create a writer that listens to the document
                    writer = PdfWriter.GetInstance(document, ms);//


                    //Step 3: Open the document
                    document.Open();//

                    PdfContentByte cb = writer.DirectContent;//

                    for (int pageNumber = 1; pageNumber < reader.NumberOfPages+1; pageNumber++)//
                    {
                        if (!pageIndexes.Contains(pageNumber))
                        {
                            document.SetPageSize(reader.GetPageSizeWithRotation(1));
                            document.NewPage();

                            //Insert to Destination on the first page
                            if (pageNumber == 1)
                            {
                                Chunk fileRef = new Chunk(" ");
                                document.Add(fileRef);
                            }

                            PdfImportedPage page = writer.GetImportedPage(reader, pageNumber);
                            int rotation = reader.GetPageRotation(pageNumber);
                            if (rotation == 90 || rotation == 270)
                            {
                                cb.AddTemplate(page, 0, -1f, 1f, 0, 0, reader.GetPageSizeWithRotation(pageNumber).Height);
                            }
                            else
                            {
                                cb.AddTemplate(page, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                    }

                    // Add a new page to the pdf file
                   // document.NewPage();
                //    Paragraph paragraph = new Paragraph();
                 //   Chunk titleChunk = new Chunk(" ");
                 //   paragraph.Add(titleChunk);
                 //   document.Add(paragraph);
                }
                finally
                {
                    document.Close();
                }

                ms.Flush();
                buffer = ms.ToArray();

                if (reader != null)
                {
                    reader.Close();
                }

                if (writer != null)
                {
                    writer.Close();
                }
            }

            return buffer;
        }

        // ВЫБРАТЬ ВСЕ МАРКИ
        // УДАЛИТЬ ВСЕ МАРКИ ДЛЯ УКАЗАННЫХ СТРАНИЦ
        // ПОМЕНЯТЬ НУМИРАЦИЮ ДЛЯ ВСЕХ МАРОК


        /// <summary>
        /// The process ellipse.
        /// </summary>
        /// <param name="dr">
        /// The dr.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="pdfReader">The reader</param>
        private void ProcessEllipse(ATShape dr, PdfStamper stamper, PdfReader pdfReader)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;

            Action<PdfContentByte, ATShape> drawAction =
                (cb, s) => cb.Ellipse(s.PositionX, pageHeight - (s.PositionY + s.Height), s.PositionX + s.Width, pageHeight - s.PositionY);

            this.ProcessShape(dr, drawAction, stamper);
        }

        /// <summary>
        /// The process rectangle.
        /// </summary>
        /// <param name="dr">
        /// The dr.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="pdfReader">The reader</param>
        private void ProcessRectangle(ATShape dr, PdfStamper stamper, PdfReader pdfReader)
        {
            var pageHeight = pdfReader.GetPageSizeWithRotation(dr.Mark.PageIndex).Height;

            Action<PdfContentByte, ATShape> drawAction = (cb, s) => cb.Rectangle(s.PositionX, pageHeight - (s.PositionY + s.Height), s.Width, s.Height);

            this.ProcessShape(dr, drawAction, stamper);
        }


        /// <summary>
        /// The process rectangle.
        /// </summary>
        /// <param name="dr">
        /// The dr.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="pdfReader">The reader</param>
        private void ProcessRules(ATShape dr, PdfStamper stamper, PdfReader pdfReader)
        {
            for (int i = 1; i <= pdfReader.NumberOfPages; i++)
            {
                var page = pdfReader.GetPageSizeWithRotation(i);
                var pageWidth = page.Width;
                var pageHeight = page.Height;
                var columnsCount = Math.Max(1, (int)dr.Width);
                int rowsCount = 0;
                var cellWidth = pageWidth / columnsCount;
                var cellHeight = 0f;

                if (dr.Style == "1") // square cells
                {
                    cellHeight = cellWidth;
                    rowsCount = (int)Math.Floor(pageHeight / cellHeight) + 1;
                }
                else
                {
                    rowsCount = Math.Max(1, (int)dr.Height);
                    cellHeight = pageHeight / rowsCount;
                }

                var cb = stamper.GetOverContent(i);
                var bc = GetColor(dr.Color);

                cb.SaveState();

                for (int column = 1; column < columnsCount; column++)
                {
                    cb.SetColorStroke(bc);
                    cb.SetLineWidth(dr.StrokeWidth);
                    cb.MoveTo(column * cellWidth, pageHeight);
                    cb.LineTo(column * cellWidth, 0);
                    cb.ClosePathStroke();
                }

                for (int row = 1; row < rowsCount; row++)
                {
                    cb.SetColorStroke(bc);
                    cb.SetLineWidth(dr.StrokeWidth);
                    cb.MoveTo(0, row * cellHeight);
                    cb.LineTo(pageWidth, row * cellHeight);
                    cb.ClosePathStroke();
                }

                cb.RestoreState();
            }
        }

        /// <summary>
        /// The process drawings.
        /// </summary>
        /// <param name="shape">
        /// The drawings.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        private void ProcessShape(ATShape shape, PdfStamper stamper, PdfReader reader)
        {
            var size = reader.GetPageSizeWithRotation(shape.Mark.PageIndex);

            Action<PdfContentByte, ATShape> drawAction = (cb, s) =>
            {
                var parsedPoints = this.ParseDrawing(shape.Points, size).ToList();
                if (parsedPoints.Any())
                {
                    var posX = s.PositionX;
                    var posY = s.PositionY;
                    if (Math.Abs(posX) > float.Epsilon || Math.Abs(posY) > float.Epsilon)
                    {
                        posY = size.Height - s.PositionY;
                    }
                    var vectors =
                        Enumerable.Range(0, (parsedPoints.Count + 1) / 2)
                                  .Select(i => new { start = parsedPoints[2 * i], end = parsedPoints.ElementAtOrDefault(2 * i + 1) });
                    Point last = null;
                    foreach (var v in vectors)
                    {
                        var start = null != v.end ? v.start : last ?? v.start;
                        var end = v.end ?? v.start;

                        if (null == last || Math.Abs(start.X - last.X) > float.Epsilon || Math.Abs(start.Y - last.Y) > float.Epsilon)
                        {
                            cb.MoveTo(posX + start.X, posY + start.Y);
                        }

                        cb.LineTo(posX + end.X, posY + end.Y);

                        last = end;
                    }
                }
            };

            this.ProcessShape(shape, drawAction, stamper);
        }

        /// <summary>
        /// The process drawings.
        /// </summary>
        /// <param name="shape">
        /// The drawings.
        /// </param>
        /// <param name="stamper">
        /// The stamper.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        private void ProcessDraw(ATShape shape, PdfStamper stamper, PdfReader reader)
        {
            var size = reader.GetPageSizeWithRotation(shape.Mark.PageIndex);

            Action<PdfContentByte, ATShape> drawAction = (cb, s) =>
            {
                var posX = s.PositionX;
                var posY = s.PositionY;
                if (Math.Abs(posX) > float.Epsilon || Math.Abs(posY) > float.Epsilon)
                {
                    posY = size.Height - s.PositionY;
                    size.Top = size.Bottom;
                }
                cb.SetLineCap(PdfContentByte.LINE_CAP_ROUND);
                cb.SetLineJoin(PdfContentByte.LINE_JOIN_ROUND);
                var parsedPoints = this.ParseDrawing(shape.Points, size).ToList();
                if (parsedPoints.Any())
                {
                    var firstPoint = parsedPoints.First();
                    cb.MoveTo(posX + firstPoint.X, posY + firstPoint.Y);
                    foreach (var point in parsedPoints)
                    {
                        cb.LineTo(posX + point.X, posY + point.Y);
                    }
                }
            };

            this.ProcessShape(shape, drawAction, stamper);
        }

        /// <summary>
        /// The parse drawing.
        /// </summary>
        /// <param name="points">
        /// The points.
        /// </param>
        /// <param name="pageSize">
        /// The page Size.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Point}"/>.
        /// </returns>
        private IEnumerable<Point> ParseDrawing(string points, Rectangle pageSize)
        {
            var result = new List<Point>();
            foreach (var point in PointsConverter.Parse(points))
            {
                {
                    var x = point.X;
                    var y = pageSize.Height - point.Y;
                    result.Add(new Point { X = x, Y = y });
                }
            }

            return result;
        }

        /// <summary>
        /// Gets color from string
        /// </summary>
        /// <param name="colorString">color string</param>
        private static BaseColor GetColor(string colorString)
        {
            var color = new BaseColor(Convert.ToInt32(colorString.TrimStart('#'), 16));
            return color;
        }
    }
}
