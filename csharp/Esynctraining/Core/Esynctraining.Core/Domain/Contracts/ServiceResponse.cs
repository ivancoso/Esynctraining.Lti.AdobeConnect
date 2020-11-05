#if NET45 || NET461

namespace Esynctraining.Core.Domain.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;

    /// <summary>
    /// The service response.
    /// </summary>
    /// <typeparam name="T">
    /// The result type
    /// </typeparam>
    [DataContract]
    [KnownType(typeof(ServiceResponse<string>))]
    public class ServiceResponse<T> : ServiceResponse
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the object.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public T @object { get; set; }

        /// <summary>
        ///     Gets or sets the objects.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public IEnumerable<T> objects { get; set; }

        /// <summary>
        ///     Gets or sets total objects count (for paging).
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public int totalCount { get; set; }

        #endregion
    }

    /// <summary>
    ///     The service response.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ServiceResponse))]
    public class ServiceResponse
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceResponse" /> class.
        /// </summary>
        public ServiceResponse()
        {
            this.status = Errors.CODE_RESULTTYPE_SUCCESS;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the error.
        /// </summary>
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public Error error { get; set; }

        /// <summary>
        ///     Gets or sets the status.
        /// </summary>
        [DataMember]
        public string status { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The set error.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SetError(Error error)
        {
            this.error = error;
            this.status = Errors.CODE_RESULTTYPE_ERROR;
        }

        /// <summary>
        /// The set error.
        /// </summary>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        public void SetError(string status, Exception error)
        {
            this.error = new Error(Errors.CODE_ERRORTYPE_GENERIC_ERROR, status, error.Message, error.ToString());
            this.status = Errors.CODE_RESULTTYPE_ERROR;
        }

        #endregion
    }
}

#endif
