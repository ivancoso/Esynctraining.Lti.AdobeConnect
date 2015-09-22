namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The state DTO.
    /// </summary>
    [DataContract]
    public class GeoStateDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="GeoStateDTO" /> class.
        /// </summary>
        public GeoStateDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoStateDTO"/> class.
        /// </summary>
        /// <param name="s">
        /// The state.
        /// </param>
        public GeoStateDTO(State s)
        {
            this.stateId = s.Id;
            this.stateCode = s.StateCode;
            this.stateName = s.StateName;
            this.isActive = s.IsActive;
            this.latitude = s.Latitude;
            this.longitude = s.Longitude;
            this.zoomLevel = s.ZoomLevel;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether is active.
        /// </summary>
        [DataMember]
        public bool isActive { get; set; }

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        [DataMember]
        public decimal latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        [DataMember]
        public decimal longitude { get; set; }

        /// <summary>
        /// Gets or sets the state code.
        /// </summary>
        [DataMember]
        public string stateCode { get; set; }

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
        /// Gets or sets the zoom level.
        /// </summary>
        [DataMember]
        public int zoomLevel { get; set; }

        #endregion
    }
}