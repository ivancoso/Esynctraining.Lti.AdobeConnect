using System.Xml;
using Esynctraining.AC.Provider.Entities;
using Esynctraining.AC.Provider.Extensions;

namespace Esynctraining.AC.Provider.EntityParsing
{
    internal static class OwaspParser
    {
        public static OWASPInfo Parse(XmlNode xml)
        {
            if (xml == null || !xml.NodeExists("//OWASP_CSRF_TOKEN"))
            {
                return null;
            }

            return new OWASPInfo() {Token = xml.SelectSingleNodeValue("//OWASP_CSRF_TOKEN/token/text()")};
        }
    }
}