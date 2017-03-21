using System.Collections.Generic;
using System.Linq;
using EdugameCloud.Core.Business;
using EdugameCloud.Lti.Core.Constants;
using EdugameCloud.Lti.Core.Domain.Entities;
using EdugameCloud.Lti.Domain.Entities;
using EdugameCloud.Lti.DTO;
using Esynctraining.NHibernate;
using Esynctraining.NHibernate.Queries;
using NHibernate;
using NHibernate.SqlCommand;
using NHibernate.Transform;

namespace EdugameCloud.Lti.Core.Business.Models
{
    /// <summary>
    /// The company LMS model.
    /// </summary>
    public sealed class LmsCompanyModel : BaseModel<LmsCompany, int>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsCompanyModel"/> class.
        /// </summary>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public LmsCompanyModel(IRepository<LmsCompany, int> repository)
            : base(repository)
        {
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get one by company id.
        /// </summary>
        /// <param name="companyId">
        /// The company id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsCompany}"/>.
        /// </returns>
        public IEnumerable<LmsCompany> GetAllByCompanyId(int companyId)
        {
            var queryOver =
                new DefaultQueryOver<LmsCompany, int>().GetQueryOver()
                    .Where(c => c.CompanyId == companyId);
            return this.Repository.FindAll(queryOver);
        }

        ///// <summary>
        ///// Gets one by domain
        ///// </summary>
        ///// <param name="domain">
        ///// The domain
        ///// </param>
        ///// <returns>
        ///// The canvas ac meeting
        ///// </returns>
        //public IFutureValue<LmsCompany> GetOneByDomain(string domain)
        //{
        //    var defaultQuery = new DefaultQueryOver<LmsCompany, int>().GetQueryOver()
        //        .Where(x => x.LmsDomain == domain).Take(1);
        //    return this.Repository.FindOne(defaultQuery);
        //}

        /// <summary>
        /// The get one by ac domain.
        /// </summary>
        /// <param name="adobeConnectDomain">
        /// The ac domain.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{LmsCompany}"/>.
        ///// </returns>
        //public IFutureValue<LmsCompany> GetOneByAcDomain(string adobeConnectDomain)
        //{
        //    if (adobeConnectDomain.EndsWith("/"))
        //    {
        //        adobeConnectDomain = adobeConnectDomain.Remove(adobeConnectDomain.Length - 1);
        //    }
        //    var defaultQuery = new DefaultQueryOver<LmsCompany, int>().GetQueryOver()
        //        .WhereRestrictionOn(x => x.AcServer).IsInsensitiveLike(adobeConnectDomain, MatchMode.Start)
        //        .Take(1);
        //    return this.Repository.FindOne(defaultQuery);
        //}

        /// <summary>
        /// The get one by provider and domain or consumer key.
        /// </summary>
        /// <param name="lmsProviderId">
        /// The provider ID.
        /// </param>
        /// <param name="consumerKey">
        /// The consumer key
        /// </param>
        /// <returns>
        /// The canvas AC meeting
        /// </returns>
        public IFutureValue<LmsCompany> GetOneByProviderAndConsumerKey(int lmsProviderId, string consumerKey)
        {
            var defaultQuery = new DefaultQueryOver<LmsCompany, int>().GetQueryOver()
                .Fetch(x => x.Settings).Eager
                .Where(x => (x.ConsumerKey == consumerKey && x.LmsProviderId == lmsProviderId));
            return this.Repository.FindOne(defaultQuery);
        }

        public IFutureValue<LmsCompany> GetOneByConsumerKey(string consumerKey)
        {
            var defaultQuery = new DefaultQueryOver<LmsCompany, int>().GetQueryOver()
                .Where(x => (x.ConsumerKey == consumerKey));
            return this.Repository.FindOne(defaultQuery);
        }

        public IEnumerable<LmsCompany> GetAllByProviderId(int providerId)
        {
            var defaultQuery = new DefaultQueryOver<LmsCompany, int>().GetQueryOver().Where(x => x.LmsProviderId == providerId);
            return this.Repository.FindAll(defaultQuery);
        }

        public void UpdateCompanySetting(LmsCompany lmsCompany, string settingName, string settingValue)
        {
            if ((lmsCompany.Id == default(int)) && (lmsCompany.Settings == null))
                lmsCompany.Settings = new List<LmsCompanySetting>();

            LmsCompanySetting setting = lmsCompany.Settings.SingleOrDefault(x => string.Compare(x.Name, settingName, true) == 0);
            if (setting == null)
            {
                lmsCompany.Settings.Add(new LmsCompanySetting
                {
                    LmsCompany = lmsCompany,
                    Name = settingName,
                    Value = settingValue,
                });
            }
            else
            {
                setting.Value = settingValue;
            }
        }

        public List<LmsCompany> GetEnabledForSynchronization(string consumerKey = null)
        {
            LmsCompany lc = null;
            LmsCompanySetting lcs = null;
            OfficeHours oh = null;
            IList<LmsCourseMeeting> lcm = null;
            IList<LmsUserMeetingRole> lumr = null;
            //IList<LmsUser> lu = null;
            //LmsUser u = null;
            var defaultQuery = new DefaultQueryOver<LmsCompany, int>()
                .GetQueryOver(() => lc)
                .Where(x => x.IsActive);
            if(!string.IsNullOrEmpty(consumerKey))
                defaultQuery = defaultQuery.Where(x => x.ConsumerKey == consumerKey);
            defaultQuery = defaultQuery
                .JoinAlias(() => lc.Settings, () => lcs)
                .Where(() => lcs.Name == LmsCompanySettingNames.UseSynchronizedUsers && lcs.Value == "True")
                .JoinAlias(() => lc.LmsCourseMeetings, () => lcm)
                .JoinAlias(() => lcm.First().OfficeHours, () => oh, JoinType.LeftOuterJoin)
//                .JoinAlias(() => lcm.First().MeetingRoles, () => lumr)
//                .Fetch(x => x.Settings).Eager
                .Fetch(x => x.LmsCourseMeetings).Eager
                .Fetch(x => x.LmsCourseMeetings.First().OfficeHours).Eager
//                .Fetch(x => x.LmsCourseMeetings.First().MeetingRoles).Eager
                .Fetch(x => x.AdminUser).Eager
                .TransformUsing(Transformers.DistinctRootEntity);
            return this.Repository.FindAll(defaultQuery).ToList();
        }

        public void ProcessLmsAdmin(LmsCompany entity, CompanyLmsDTO resultDto, LmsUserModel lmsUserModel, LmsCompanyModel lmsCompanyModel)
        {
            if (entity.LmsProviderId == (int)LmsProviderEnum.Canvas)
                return;
            if (entity.LmsProviderId == (int)LmsProviderEnum.Desire2Learn)
                return;

            if (!resultDto.enableProxyToolMode)
            {
                var lmsUser = entity.AdminUser ?? new LmsUser { LmsCompany = entity, UserId = "0" };

                lmsUser.Username = resultDto.lmsAdmin;
                if (!string.IsNullOrEmpty(resultDto.lmsAdminPassword))
                {
                    lmsUser.Password = resultDto.lmsAdminPassword;
                }

                lmsUser.Token = resultDto.lmsAdminToken;

                lmsUserModel.RegisterSave(lmsUser, true);
                entity.AdminUser = lmsUser;
                lmsCompanyModel.RegisterSave(entity);
            }
        }

        public void DeleteWithDependencies(int lmsCompanyId)
        {
            var query = this.Repository.Session.GetNamedQuery("deleteLmsCompanyWithDependencies");
            query.SetInt32("lmsCompanyId", lmsCompanyId);
            query.UniqueResult();

            CachePolicies.InvalidateCache(CachePolicies.Dependencies.CompanyLmsSettings(lmsCompanyId));
        }

        public override void RegisterDelete(LmsCompany entity, bool flush)
        {
            base.RegisterDelete(entity, flush);

            CachePolicies.InvalidateCache(CachePolicies.Dependencies.CompanyLmsSettings(entity.Id));
        }

        public override void RegisterDelete(LmsCompany entity)
        {
            base.RegisterDelete(entity);

            CachePolicies.InvalidateCache(CachePolicies.Dependencies.CompanyLmsSettings(entity.Id));
        }

        public override void RegisterSave(LmsCompany entity, bool flush, bool updateDateModified = true)
        {
            base.RegisterSave(entity, flush, updateDateModified);

            CachePolicies.InvalidateCache(CachePolicies.Dependencies.CompanyLmsSettings(entity.Id));
        }

        public override void RegisterSave(LmsCompany entity)
        {
            base.RegisterSave(entity);

            CachePolicies.InvalidateCache(CachePolicies.Dependencies.CompanyLmsSettings(entity.Id));
        }

        #endregion

    }

}