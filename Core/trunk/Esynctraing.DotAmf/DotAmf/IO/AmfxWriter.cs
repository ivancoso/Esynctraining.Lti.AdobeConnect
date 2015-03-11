// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="AmfxWriter.cs">
//   
// </copyright>
// <summary>
//   AMFX writer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace DotAmf.IO
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;

    /// <summary>
    ///     AMFX writer.
    /// </summary>
    public abstract class AmfxWriter
    {
        #region Public Methods and Operators

        /// <summary>
        /// Create an AMFX writer for the stream.
        /// </summary>
        /// <param name="stream">
        /// The stream.
        /// </param>
        /// <param name="handleDispose">
        /// The handle Dispose.
        /// </param>
        /// <returns>
        /// The <see cref="XmlWriter"/>.
        /// </returns>
        public static XmlWriter Create(Stream stream, bool handleDispose = false)
        {
            if (stream == null)
            {
                throw new ArgumentNullException("stream");
            }

            var settings = new XmlWriterSettings
                               {
                                   CheckCharacters = false, 
                                   Encoding = Encoding.UTF8, 
                                   CloseOutput = handleDispose, 
                                   NewLineHandling = NewLineHandling.None, 
                                   OmitXmlDeclaration = true
                               };

            return XmlWriter.Create(stream, settings);
        }

        #endregion
    }
}