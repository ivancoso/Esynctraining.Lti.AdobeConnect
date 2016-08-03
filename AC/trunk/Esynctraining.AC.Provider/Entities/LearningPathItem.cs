namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    public class LearningPathItem
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the current SCO id.
        /// </summary>
        [XmlAttribute("current-sco-id")]
        public string CurrentScoId { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path type.
        /// </summary>
        [XmlAttribute("path-type")]
        public string PathType { get; set; }

        /// <summary>
        /// Gets or sets the target SCO id.
        /// </summary>
        [XmlAttribute("target-sco-id")]
        public string TargetScoId { get; set; }

        /// <summary>
        /// Gets or sets the curriculum id.
        /// </summary>
        [XmlAttribute("curriculum-id")]
        public string CurriculumId { get; set; }

        #endregion
    }
}