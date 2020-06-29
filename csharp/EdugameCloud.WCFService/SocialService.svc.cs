namespace EdugameCloud.WCFService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
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

        /// <summary>
        /// Gets the subscription history log model.
        /// </summary>
        private SubscriptionHistoryLogModel SubscriptionHistoryLogModel
        {
            get
            {
                return IoC.Resolve<SubscriptionHistoryLogModel>();
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
        /// The <see cref="string"/>.
        /// </returns>
        public string Proxy(WebRequestDTO dto)
        {
            var webProxyModel = this.WebProxyModel;
            bool success;
            ServicePointManager.Expect100Continue = false;
            var res = dto.isGetMethod ? webProxyModel.Get(dto, out success) : webProxyModel.Post(dto, out success);
            if (success)
            {
                return res;
            }

            var error = new Error(Errors.CODE_ERRORTYPE_REQUEST_NOT_PROCESSED, "WebError", "Web request failed", res);
            this.LogError("Social.Proxy", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionResDTO"/>.
        /// </returns>
        public SubscriptionResDTO SubscribeToTag(string tag)
        {
            var result = this.WebProxyModel.SubscribeToInstagramTag(tag);
            Error error;
            if (result.meta != null && result.meta.code == 200 && result.data != null)
            {
                if (result.data.id != 0)
                {
                    var item = this.SubscriptionHistoryLogModel.GetOneByTag(tag).Value;
                    item = item ?? new SubscriptionHistoryLog { SubscriptionTag = tag };
                    item.SubscriptionId = result.data.id;
                    this.SubscriptionHistoryLogModel.RegisterSave(item, true);
                    return result;
                }

                error = new Error(Errors.CODE_ERRORTYPE_INVALID_ACCESS, "Instagram id not parsed", result.raw);
            }
            else
            {
                error = new Error(result.meta.With(x => x.code), "Instagram error", result.raw);
            }

            this.LogError("Social.SubscribeToTag", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="tag">
        /// The tag.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionResDTO"/>.
        /// </returns>
        public SubscriptionResDTO UnsubscribeTag(string tag)
        {
            return this.WebProxyModel.UnsubscribeInstagramTag(tag);
        }

        /// <summary>
        /// The get updates for tag.
        /// </summary>
        /// <param name="tagList">
        /// The tag List.
        /// </param>
        /// <returns>
        /// The <see cref="SubscriptionUpdateSumDTO"/>.
        /// </returns>
        public SubscriptionUpdateSumDTO[] GetUpdatesForTags(TagRequestDTO[] tagList)
        {
            tagList = tagList ?? new TagRequestDTO[] { };
            this.SubscriptionHistoryLogModel.SaveUpdate(tagList.Select(x => new Tuple<string, int>(x.tag, 0)).ToList());
            var res = this.SubscriptionUpdateModel.GetAllByTags(tagList.ToList()).ToList();
            var groupedList = tagList.ToDictionary(tag => tag.tag, tag => new Tuple<long, List<SubscriptionUpdate>>(tag.time, res.Where(x => x.Object_id == tag.tag).ToList()));
            return groupedList.Select(x => new SubscriptionUpdateSumDTO(x.Key, x.Value.Item2, x.Value.Item1)).ToArray();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <returns>
        /// The <see cref="SubscriptionDTO"/>.
        /// </returns>
        public SubscriptionDTO ListSubscriptions()
        {
            return this.WebProxyModel.ListSubscriptions();
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="SocialUserTokensDTO"/>.
        /// </returns>
        public SocialUserTokensDTO GetSocialUserTokens(string key)
        {
            SocialUserTokens user;
            if ((user = this.SocialUserTokensModel.GetOneByKey(key).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("SocialUserTokens.GetSocialUserTokens", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new SocialUserTokensDTO(user);
        }

        #endregion
    }
}