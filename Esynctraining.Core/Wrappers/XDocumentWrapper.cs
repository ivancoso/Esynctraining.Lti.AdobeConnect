namespace Esynctraining.Core.Wrappers
{
    using System.Xml.Linq;

    /// <summary>
    /// The x document wrapper.
    /// </summary>
    public class XDocumentWrapper
    {
        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="XDocument"/>.
        /// </returns>
        public virtual XDocument Load(string path)
        {
            return XDocument.Load(path);
        }
    }
}
