namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    public class EventLoginInfo
    {
        [XmlAttribute("entry-url")]
        public string EntryUrl { get; set; }
      
    }
}
