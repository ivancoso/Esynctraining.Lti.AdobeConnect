namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// One field describing the principal, account, or object.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// The name of the field.
        /// </summary>
        [XmlAttribute("field-id")]
        public string FieldId { get; set; }

        /// <summary>
        /// The value of the field.
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }

    }

}
