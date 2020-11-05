namespace PDFAnnotation.Core.Domain.DTO
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// The lookup dto.
    /// </summary>
    [DataContract]
    public class LookupDTO
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the contact types.
        /// </summary>
        [DataMember]
        public List<ContactTypeDTO> ContactTypes { get; set; }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [DataMember]
        public List<CountryDTO> Countries { get; set; }

        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        [DataMember]
        public List<StateDTO> States { get; set; }

        #endregion
    }
}