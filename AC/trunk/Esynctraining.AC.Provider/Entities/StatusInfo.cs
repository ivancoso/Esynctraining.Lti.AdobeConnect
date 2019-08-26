namespace Esynctraining.AC.Provider.Entities
{
    using System;
    using System.Xml;
    using Extensions;

    /// <summary>
    /// StatusInfo structure, holds status information during API calls
    /// </summary>
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
            this.Type = string.Empty;
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Type { get; set; }

        public int? Min { get; set; }

        public int? Max { get; set; }

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

        /// <summary>
        /// User suspended period (in minutes).
        /// </summary>
        public int? UserSuspendedParam { get; set; }


        public int? TryGetSubCodeAsInt32()
        {
            if (string.IsNullOrWhiteSpace(InnerXml))
                return null;

            var doc = new XmlDocument();
            doc.LoadXml(InnerXml);
            var subCode = doc.SelectSingleNodeValue("//status/@subcode");
            int value;
            if (int.TryParse(subCode, out value))
            {
                return value;
            }

            return null;
        }

    }

}
