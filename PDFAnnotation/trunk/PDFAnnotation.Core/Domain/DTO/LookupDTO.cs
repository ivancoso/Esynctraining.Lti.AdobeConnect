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

        private ContactTypeDTO[] _contactTypes;
        private CountryDTO[] _countries;
        private StateDTO[] _states;

        #region Public Properties

        /// <summary>
        /// Gets or sets the contact types.
        /// </summary>
        [DataMember]
        public ContactTypeDTO[] ContactTypes
        {
            get
            {
                return this._contactTypes ?? new ContactTypeDTO[] { };
            }

            set
            {
                this._contactTypes = value;
            }
        }

        /// <summary>
        /// Gets or sets the countries.
        /// </summary>
        [DataMember]
        public CountryDTO[] Countries
        {
            get
            {
                return this._countries ?? new CountryDTO[] { };
            }

            set
            {
                this._countries = value;
            }
        }

        /// <summary>
        /// Gets or sets the states.
        /// </summary>
        [DataMember]
        public StateDTO[] States
        {
            get
            {
                return this._states ?? new StateDTO[] { };
            }

            set
            {
                this._states = value;
            }
        }

        #endregion
    }
}