namespace PDFAnnotation.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Extensions;

    using PDFAnnotation.Core.Domain.Entities;

    /// <summary>
    ///     The state DTO.
    /// </summary>
    [DataContract]
    [Serializable]
    public class StateDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StateDTO" /> class.
        /// </summary>
        public StateDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateDTO"/> class.
        /// </summary>
        /// <param name="s">
        /// The state.
        /// </param>
        public StateDTO(State s)
        {
            this.stateId = s.Id;
            this.stateCode = s.StateCode;
            this.stateName = s.StateName;
            this.isActive = s.IsActive;
            this.countryId = s.Country.With(x => x.Id);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the state id.
        /// </summary>
        [DataMember]
        public int stateId { get; set; }

        /// <summary>
        /// Gets or sets the state name.
        /// </summary>
        [DataMember]
        public string stateName { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        [DataMember]
        public string stateCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the country id.
        /// </summary>
        [DataMember]
        public int? countryId { get; set; }

        #endregion
    }
}