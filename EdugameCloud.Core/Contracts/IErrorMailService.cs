using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdugameCloud.Core.Contracts
{
    using System.ServiceModel;

    using Esynctraining.Core.Domain.Contracts;

    /// <summary>
    ///     The AccountService interface.
    /// </summary>
    [ServiceContract]
    public interface IErrorMailService
    {
        #region Public Methods and Operators

        /// <summary>
        /// The all.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        [OperationContract]
        ServiceResponse SendMailAboutError(string message, string details);

        #endregion
    }
}
