namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// MeetingUpdateItem structure
    /// </summary>
    [XmlRoot("meeting")]
    public class MeetingUpdateItem : ScoUpdateItemBase
    {
        /// <summary>
        /// Gets or sets the icon.
        /// </summary>
        [XmlAttribute("icon")]
        public string Icon { get; set; }

    }

}
