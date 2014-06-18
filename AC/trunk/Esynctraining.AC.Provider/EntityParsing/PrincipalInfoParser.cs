namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;

    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    ///     The Principal Info parser.
    /// </summary>
    internal static class PrincipalInfoParser
    {
        #region Constants

        /// <summary>
        /// The contact path.
        /// </summary>
        private const string ContactPath = "//contact";

        /// <summary>
        /// The manager path.
        /// </summary>
        private const string ManagerPath = "//manager";

        /// <summary>
        /// The preferences path.
        /// </summary>
        private const string PreferencesPath = "//preferences";

        /// <summary>
        ///     The path.
        /// </summary>
        private const string PrincipalPath = "//principal";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Parses the specified XML.
        /// </summary>
        /// <param name="xml">
        /// The XML.
        /// </param>
        /// <returns>
        /// Collection of Meeting Items.
        /// </returns>
        public static PrincipalInfo Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeExists(PrincipalPath))
            {
                TraceTool.TraceMessage(string.Format("Node {0} is empty: no data available", PrincipalPath));
                return null;
            }

            return new PrincipalInfo
                       {
                           Contact = xml.NodeExists(ContactPath) ? ContactParser.Parse(xml.SelectSingleNode(ContactPath)) : null,
                           PrincipalPreferences = xml.NodeExists(PreferencesPath) ? PrincipalPreferencesParser.Parse(xml.SelectSingleNode(PreferencesPath)) : null,
                           Principal = PrincipalParser.Parse(xml.SelectSingleNode(PrincipalPath)),
                           Manager = xml.NodeExists(ManagerPath) ? PrincipalParser.Parse(xml.SelectSingleNode(ManagerPath)) : null,
                       };
        }

        #endregion
    }
}