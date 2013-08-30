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

        ///// <summary>
        ///// User Login.
        ///// Used to log the User in if SessionId is not defined or login by SessionId failed.
        ///// </summary>
        //public string Login { get; set; }

        ///// <summary>
        ///// User password.
        ///// Used to log the User in if SessionId is not defined or login by SessionId failed.
        ///// </summary>
        //public string Password { get; set; }

        ///// <summary>
        ///// Session ID is used for SSO (Single-Sign-On).
        ///// If login with Session ID fails, User Login and Password will be used to obtain a new Session ID
        ///// </summary>
        //public string SessionId { get; set; }
        
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
