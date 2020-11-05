namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// EventUpdateItem structure
    /// </summary>
    [XmlRoot("event")]
    public class EventUpdateItem : ScoUpdateItemBase
    {
        /// <summary>
        /// Gets or sets the event info.
        /// </summary>
        [XmlElement("event-info")]
        public string EventInfo { get; set; }
    }
}
