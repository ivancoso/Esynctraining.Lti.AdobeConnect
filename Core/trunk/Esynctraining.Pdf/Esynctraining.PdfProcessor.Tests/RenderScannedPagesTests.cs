namespace Esynctraining.PdfProcessor.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Esynctraining.PdfProcessor.Actions;
    using Esynctraining.PdfProcessor.Actions.Filters;
    using Esynctraining.PdfProcessor.Renders;

    using iTextSharp.text.pdf;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     The render scanned pages tests.
    /// </summary>
    [TestClass]
    public class RenderScannedPagesTests
    {
        #region Constants

        /// <summary>
        ///     The resources.
        /// </summary>
        private const string Resources = "Esynctraining.PdfProcessor.Tests.Resources.";

        #endregion

        #region Static Fields

        /// <summary>
        ///     The target.
        /// </summary>
        private static PdfProcessorHelper target;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialization.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        [ClassInitialize]
        public static void Init(TestContext context)
        {
            target = new PdfProcessorHelper();
        }

        /// <summary>
        ///     The tear down.
        /// </summary>
        [ClassCleanup]
        public static void TearDown()
        {
            string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.pdf");
            foreach (string file in files)
            {
                File.Delete(file);
            }
        }

        /// <summary>
        ///     Verifies that internal logic with actions works fine
        /// </summary>
        [TestMethod]
        public void CheckActions_ResultedFileContainsOnlyImages_ProperlyRotated()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectangles1.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectangles.pdf", inPdfPath);
            string outPdfPath = inPdfPath + ".out.pdf";
            using (var reader = new PdfReader(inPdfPath))
            {
                using (var fs = new FileStream(outPdfPath, FileMode.Create))
                {
                    using (var stamper = new PdfStamper(reader, fs))
                    {
                        using (var render = new GsNetPdfRender(inPdfPath, 150))
                        {
                            var andFilter = new AndFilter(
                                new HasSpecialCsResourceFilter(reader), 
                                new NotFilter(new HasFontResourceFilter(reader)), 
                                new NotFilter(
                                    new PageHasSpecificKeyFilter(
                                        reader, 
                                        RenderPageAction.EstPageRendered, 
                                        new PdfBoolean(true))));
                            using (var action = new RenderPageAction(reader, stamper, andFilter, render))
                            {
                                var processor = new PdfProcessor(new List<IPdfAction> { action });
                                for (int i = 1; i <= reader.NumberOfPages; i++)
                                {
                                    processor.Execute(i);
                                }
                            }
                        }
                    }
                }
            }

            using (var reader = new PdfReader(outPdfPath))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertOnlyImageAndImageSize(reader, 2, 1275, 1650);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that document had scanned pages and was fixed
        /// </summary>
        [TestMethod]
        public void CheckIfDocumentWasScannedAndFixIfNecessary_File_DocumentWasFixed()
        {
            string pdfPath = Directory.GetCurrentDirectory() + "/BlackRectangles4.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectangles.pdf", pdfPath);
            DateTime lastWrite = File.GetLastWriteTime(pdfPath);
            target.CheckIfDocumentWasScannedAndFixIfNecessary(pdfPath);
            bool wasModified = lastWrite != File.GetLastWriteTime(pdfPath);
            Assert.IsTrue(wasModified); // file was modified
            using (var reader = new PdfReader(pdfPath))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertOnlyImageAndImageSize(reader, 2, 1275, 1650);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that check if document was scanned and fix if necessary resulted buffer not changed.
        /// </summary>
        [TestMethod]
        public void CheckIfDocumentWasScannedAndFixIfNecessary_ResultedBufferNotChanged()
        {
            byte[] buffer = PdfProcessorTests.GetResourceBytes(Resources + "BlackRectanglesWithRenderedKey.pdf");
            byte[] result = target.CheckIfDocumentWasScannedAndFixIfNecessary(buffer);
            Assert.IsTrue(result.Length == buffer.Length);
        }

        /// <summary>
        /// Verifies that check if document was scanned and fix if necessary resulted file contains only images properly rotated.
        /// </summary>
        [TestMethod]
        public void CheckIfDocumentWasScannedAndFixIfNecessary_ByteArray_ResultedFileContainsOnlyImages_ProperlyRotated()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectangles3.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectangles.pdf", inPdfPath);
            byte[] buffer = File.ReadAllBytes(inPdfPath);
            byte[] result = target.CheckIfDocumentWasScannedAndFixIfNecessary(buffer);

            Assert.IsTrue(result.Length != buffer.Length); // was modified

            using (var reader = new PdfReader(result))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertOnlyImageAndImageSize(reader, 2, 1275, 1650);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that check if document was scanned and fix if necessary resulted file contains only images properly rotated.
        /// </summary>
        [TestMethod]
        public void CheckIfDocumentWasScannedAndFixIfNecessary_Stream_ResultedFileContainsOnlyImages_ProperlyRotated()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectangles5.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectangles.pdf", inPdfPath);
            byte[] buffer = File.ReadAllBytes(inPdfPath);
            var result = target.CheckIfDocumentWasScannedAndFixIfNecessary(new MemoryStream(buffer));

            Assert.IsTrue(result.Length != buffer.Length); // was modified

            using (var reader = new PdfReader(result))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertOnlyImageAndImageSize(reader, 2, 1275, 1650);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that render with GS net fix is working only for pages without text, result properly rotated.
        /// </summary>
        [TestMethod]
        public void RenderWithGsNet_FixIsWorkingOnlyForPagesWithoutText_ProperlyRotated()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectanglesWithSomeText.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectanglesWithSomeText.pdf", inPdfPath);
            string outPdfPath = inPdfPath + ".out.pdf";
            Assert.IsTrue(target.RenderScannedPagesWithGsNet(inPdfPath, outPdfPath, 150));

            using (var reader = new PdfReader(outPdfPath))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertFontResource(reader, 2);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that render with GS net test resulted file contains only images properly rotated.
        /// </summary>
        [TestMethod]
        public void RenderWithGsNetTest_ResultedFileContainsOnlyImages_ProperlyRotated()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectangles2.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectangles.pdf", inPdfPath);
            string outPdfPath = inPdfPath + ".out.pdf";
            Assert.IsTrue(target.RenderScannedPagesWithGsNet(inPdfPath, outPdfPath, 150));

            using (var reader = new PdfReader(outPdfPath))
            {
                AssertPageRotation(reader, 1, 0);
                AssertOnlyImageAndImageSize(reader, 1, 1275, 1650);
                AssertPageRotation(reader, 2, 90);
                AssertOnlyImageAndImageSize(reader, 2, 1275, 1650);
                AssertPageRotation(reader, 3, 180);
                AssertOnlyImageAndImageSize(reader, 3, 1275, 1650);
            }
        }

        /// <summary>
        /// Verifies that render with GS net test with rendered key over pages resulted file not changed.
        /// </summary>
        [TestMethod]
        public void RenderWithGsNetTest_WithRenderedKeyOverPages_ResultedFileNotChanged()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectanglesWithRenderedKey.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectanglesWithRenderedKey.pdf", inPdfPath);
            string outPdfPath = inPdfPath + ".out.pdf";
            Assert.IsFalse(target.RenderScannedPagesWithGsNet(inPdfPath, outPdfPath, 150));
        }

        /// <summary>
        /// Verifies that render with GS net test with text over pages resulted file not changed.
        /// </summary>
        [TestMethod]
        public void RenderWithGsNetTest_WithTextOverPages_ResultedFileNotChanged()
        {
            string inPdfPath = Directory.GetCurrentDirectory() + "/BlackRectanglesWithAllText.pdf";
            PdfProcessorTests.FlushResourceToFile(Resources + "BlackRectanglesWithAllText.pdf", inPdfPath);
            string outPdfPath = inPdfPath + ".out.pdf";
            Assert.IsFalse(target.RenderScannedPagesWithGsNet(inPdfPath, outPdfPath, 150));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The assert font resource.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="pageNum">
        /// The page number.
        /// </param>
        private static void AssertFontResource(PdfReader reader, int pageNum)
        {
            PdfDictionary page = reader.GetPageNRelease(pageNum);
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            PdfDictionary font = resources.GetAsDict(PdfName.FONT);
            Assert.IsNotNull(font);
        }

        /// <summary>
        /// The assert only image and image size.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="pageNum">
        /// The page number.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        private static void AssertOnlyImageAndImageSize(PdfReader reader, int pageNum, int width, int height)
        {
            PdfDictionary page = reader.GetPageNRelease(pageNum);
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            PdfDictionary dictionary = resources.GetAsDict(PdfName.XOBJECT);
            Assert.AreEqual(1, dictionary.Keys.Count);
            dictionary.GetEnumerator().MoveNext();
            Dictionary<PdfName, PdfObject>.Enumerator e = dictionary.GetEnumerator();
            e.MoveNext();
            PdfName key = e.Current.Key;
            PdfStream str = dictionary.GetAsStream(key);
            Assert.AreEqual(width, str.GetAsNumber(PdfName.WIDTH).IntValue);
            Assert.AreEqual(height, str.GetAsNumber(PdfName.HEIGHT).IntValue);
        }

        /// <summary>
        /// The assert page rotation.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="pageNum">
        /// The page number.
        /// </param>
        /// <param name="rotation">
        /// The rotation.
        /// </param>
        private static void AssertPageRotation(PdfReader reader, int pageNum, int rotation)
        {
            Assert.AreEqual(rotation, reader.GetPageRotation(pageNum));
        }

        #endregion
    }
}