namespace EdugameCloud.Core.Domain.Formats.WebEx
{
    using System.Xml.Serialization;

    /// <summary>
    /// Represents human readable bool value.
    /// </summary>
    public enum HumalBool
    {
        /// <summary>
        /// True value.
        /// </summary>
        [XmlEnum(Name = "yes")]
        Yes,
        /// <summary>
        /// False value.
        /// </summary>
        [XmlEnum(Name = "no")]
        No,
    }
}