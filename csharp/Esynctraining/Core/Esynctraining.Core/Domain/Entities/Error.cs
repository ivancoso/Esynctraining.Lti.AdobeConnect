namespace Esynctraining.Core.Domain.Entities
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The error.
    /// </summary>
    [DataContract]
    public class Error
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        public Error()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public Error(int code, string type, string message, string details = null)
        {
            this.errorCode = code;
            this.errorType = type;
            this.errorMessage = message;
            this.errorDetail = details;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Error"/> class.
        /// </summary>
        /// <param name="code">
        /// The code.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="faultEntities">
        /// The fault entities.
        /// </param>
        /// <param name="details">
        /// The details.
        /// </param>
        public Error(int code, string type, string message, List<string> faultEntities, string details = null) : this(code, type, message, details)
        {
            this.faultEntities = faultEntities;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [DataMember]
        public virtual int errorCode { get; set; }

        /// <summary>
        /// Gets or sets the error detail.
        /// </summary>
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public virtual string errorDetail { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [DataMember]
        public virtual string errorMessage { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        [DataMember]
        public virtual string errorType { get; set; }

        /// <summary>
        /// Gets or sets the fault entities.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public virtual List<string> faultEntities { get; set; }

        #endregion
    }
}