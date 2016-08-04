namespace Esynctraining.AC.Provider.DataObjects
{
    /// <summary>
    /// Connection Details for Adobe Connect API
    /// </summary>
    public class ConnectionDetails
    {
        /// <summary>
        /// Service URL. Required!
        /// </summary>
        public string ServiceUrl { get; set; }
        
        /// <summary>
        /// Proxy Credentials (optional).
        /// </summary>
        public ProxyCredentials Proxy { get; set; }

        /// <summary>
        /// Event maximum participants.
        /// </summary>
        public int EventMaxParticipants { get; set; }
    }
}
