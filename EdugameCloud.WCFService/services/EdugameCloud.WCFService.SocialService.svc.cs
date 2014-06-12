// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Contracts;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;

    using Esynctraining.Core.Domain.Contracts;
    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using Resources;

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
        ///     Gets the social user tokens
        /// </summary>
        protected SocialUserTokensModel SocialUserTokensModel
        {
            get
            {
                return IoC.Resolve<SocialUserTokensModel>();
            }
        }

        /// <summary>
        ///     Gets the social user tokens
        /// </summary>
        protected SubscriptionUpdateModel SubscriptionUpdateModel
        {
            get
            {
                return IoC.Resolve<SubscriptionUpdateModel>();
            }
        }

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

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubscriptionResDTO> SubscribeToTag(string tag)
        {
            return new ServiceResponse<SubscriptionResDTO>
                       {
                           @object = this.WebProxyModel.SubscribeToInstagramTag(tag)
                       };
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubscriptionResDTO> UnsubscribeTag(string tag)
        {
            return new ServiceResponse<SubscriptionResDTO>
            {
                @object = this.WebProxyModel.UnsubscribeInstagramTag(tag)
            };
        }

        /// <summary>
        /// The get updates for tag.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubscriptionUpdateDTO> GetUpdatesForTag(string tag)
        {
            return new ServiceResponse<SubscriptionUpdateDTO>
            {
                @objects = this.SubscriptionUpdateModel.GetAllByTag(tag).ToList().Select(x => new SubscriptionUpdateDTO(x)).ToList()
            };
        }


        /// <summary>
        /// The get by id.
        /// </summary>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SubscriptionDTO> ListSubscriptions()
        {
            return new ServiceResponse<SubscriptionDTO>
            {
                @object = this.WebProxyModel.ListSubscriptions()
            };
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<SocialUserTokensDTO> GetSocialUserTokens(string key)
        {
            var result = new ServiceResponse<SocialUserTokensDTO>();
            SocialUserTokens user;
            if ((user = this.SocialUserTokensModel.GetOneByKey(key).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_USER,
                        ErrorsTexts.AccessError_Subject,
                        ErrorsTexts.GetById_NoUserExists));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new SocialUserTokensDTO(user);
            }

            return result;
        }

        #endregion
    }
}