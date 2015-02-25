// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfxReader.cs">
//   
// </copyright>
// <summary>
//   AMFX reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.IO
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
    ///     AMFX reader.
    /// </summary>
    public abstract class AmfxReader
    {
        #region Public Methods and Operators

        /// <summary>
        /// Create an AMFX reader for the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="handleDispose">
        /// The handle Dispose.
        /// </param>
        /// <returns>
        /// The <see cref="XmlReader"/>.
        /// </returns>
        public static XmlReader Create(Stream stream, bool handleDispose = false)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var settings = new XmlReaderSettings
                               {
                                   DtdProcessing = DtdProcessing.Ignore, 
                                   IgnoreProcessingInstructions = true, 
                                   ValidationFlags = XmlSchemaValidationFlags.None, 
                                   ValidationType = ValidationType.None, 
                                   CloseInput = handleDispose, 
                                   CheckCharacters = false, 
                                   IgnoreComments = true, 
                                   IgnoreWhitespace = true
                               };

            return XmlReader.Create(stream, settings);
        }

        #endregion
    }
}