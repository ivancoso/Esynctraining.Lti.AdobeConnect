// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession, 
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SocialService : BaseService, ISocialService
    {
        #region Properties

        /// <summary>
        ///     Gets the build version Model.
        /// </summary>
        private WebProxyModel WebProxyModel
        {
            get
            {
                return IoC.Resolve<WebProxyModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert to PDF.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<string> Proxy(WebRequestDTO dto)
        {
            var result = new ServiceResponse<string>();
            var webProxyModel = this.WebProxyModel;
            bool success;
            ServicePointManager.Expect100Continue = false;
            var res = dto.isGetMethod ? webProxyModel.Get(dto, out success) : webProxyModel.Post(dto, out success);
            if (success)
            {
                result.@object = res;
            }
            else
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, "WebError", "Web request failed", res));
            }

            return result;
        }

        #endregion
    }
}