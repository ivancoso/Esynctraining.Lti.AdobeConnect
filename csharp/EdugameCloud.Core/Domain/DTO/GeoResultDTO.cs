namespace EdugameCloud.Core.Domain.DTO
{
    using System.Runtime.Serialization;

    /// <summary>
    /// The geo DTO.
    /// </summary>
    [DataContract]
    public class GeoResultDTO
    {
        #region Public Properties

        /// <summary>
        ///   Gets or sets the latitude
        /// </summary>
        [DataMember]
        public double latitude { get; set; }

        /// <summary>
        ///   Gets or sets the  longitude
        /// </summary>
        [DataMember]
        public double longitude { get; set; }

        #endregion
    }
}