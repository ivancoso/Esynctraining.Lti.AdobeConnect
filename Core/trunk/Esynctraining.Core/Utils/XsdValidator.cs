#if NET45 || NET461

namespace Esynctraining.Core.Utils
{
    using System;
    using System.Xml.Linq;
    using System.Xml.Schema;

    /// <summary>
    /// The xsd validator.
    /// </summary>
    public static class XsdValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validate xml agains xsd.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <param name="xsdFileName">
        /// The xsd file name.
        /// </param>
        /// <param name="validationError">
        /// The validation error.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool ValidateXmlAgainsXsd(string xml, string xsdFileName, out string validationError)
        {
            validationError = null;
            try
            {
                var schemas = new XmlSchemaSet();
                schemas.Add(null, xsdFileName);
                XDocument doc = XDocument.Parse(xml);
                string msg = string.Empty;
                doc.Validate(schemas, (o, e) => { msg = e.Message; });
                bool valid = string.IsNullOrWhiteSpace(msg);
                if (!valid)
                {
                    validationError = msg;
                }

                return valid;
            }
            catch (Exception ex)
            {
                validationError = ex.ToString();
                return false;
            }
        }

        #endregion
    }
}

#endif