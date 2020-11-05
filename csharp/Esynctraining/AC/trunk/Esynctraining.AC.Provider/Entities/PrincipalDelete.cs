namespace Esynctraining.AC.Provider.Entities
{
    using System.Xml.Serialization;

    /// <summary>
    /// PrincipalSetup structure
    /// </summary>
    public class PrincipalDelete
    {
        /// <summary>
        /// The ID of the principal that has
        /// information you want to delete. Required
        /// to update a user or group, but do not use
        /// to create either.
        /// </summary>
        [XmlElement("principal-id")]
        public string PrincipalId { get; set; }
    }
}
