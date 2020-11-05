using System.Xml.Serialization;

namespace Esynctraining.AC.Provider.Entities
{
    public class OWASPInfo
    {
        [XmlElement("token")]
        public string Token { get; set; }
    }
}