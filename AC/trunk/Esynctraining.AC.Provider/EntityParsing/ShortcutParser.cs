namespace Esynctraining.AC.Provider.EntityParsing
{
    using System.Xml;
    using Esynctraining.AC.Provider.Entities;
    using Esynctraining.AC.Provider.Extensions;

    /// <summary>
    /// The Shortcut parser.
    /// </summary>
    internal static class ShortcutParser
    {
        /// <summary>
        /// The parse.
        /// </summary>
        /// <param name="xml">
        /// The xml.
        /// </param>
        /// <returns>
        /// The <see cref="ScoShortcut"/>.
        /// </returns>
        public static ScoShortcut Parse(XmlNode xml)
        {
            if (xml == null || xml.Attributes == null)
            {
                return null;
            }

            return new ScoShortcut
                       {
                           ScoId = xml.SelectAttributeValue("sco-id"),
                           TreeId = xml.ParseAttributeInt("tree-id"),
                           Type = xml.SelectAttributeValue("type"),
                           DomainName = xml.SelectSingleNodeValue("domain-name/text()")
                       };
        }
    }
}
