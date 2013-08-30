namespace Esynctraining.AC.Provider.Entities
{
    using System;

    /// <summary>
    /// StatusInfo structure, holds status information during API calls
    /// </summary>
    [Serializable]
    public class StatusInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatusInfo"/> class.
        /// </summary>
        public StatusInfo()
        {
            this.Code = StatusCodes.not_set;
            this.SubCode = StatusSubCodes.not_set;
            this.InnerXml = string.Empty;
            this.SessionInfo = string.Empty;
            this.InvalidField = string.Empty;
        }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        public StatusCodes Code { get; set; }

        /// <summary>
        /// Gets or sets the sub code.
        /// </summary>
        public StatusSubCodes SubCode { get; set; }

        /// <summary>
        /// Gets or sets the invalid field.
        /// </summary>
        public string InvalidField { get; set; }

        /// <summary>
        /// Gets or sets the underlying exception info.
        /// </summary>
        public Exception UnderlyingExceptionInfo { get; set; }

        /// <summary>
        /// Gets or sets the inner xml.
        /// </summary>
        public string InnerXml { get; set; }

        /// <summary>
        /// Gets or sets the session info.
        /// </summary>
        public string SessionInfo { get; set; }
    }
}
