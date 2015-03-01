namespace EdugameCloud.Core.Business.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using Esynctraining.Core.Business;
    using Esynctraining.Core.Business.Models;
    using Esynctraining.Core.Business.Queries;

    using NHibernate;
    using NHibernate.Criterion;
    using NHibernate.SqlCommand;
    using NHibernate.Transform;

    /// <summary>
    ///     The SN profile model.
    /// </summary>
    public class SNProfileModel : BaseModel<SNProfile, int>
    {
        /// <summary>
        /// The user model.
        /// </summary>
        private readonly UserModel userModel;

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SNProfileModel"/> class.
        /// </summary>
        /// <param name="userModel">
        /// The user Model.
        /// </param>
        /// <param name="repository">
        /// The repository.
        /// </param>
        public SNProfileModel(UserModel userModel, IRepository<SNProfile, int> repository)
            : base(repository)
        {
            this.userModel = userModel;
        }

        #endregion

        /// <summary>
        /// The get one by SMI id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="IFutureValue{SNProfile}"/>.
        /// </returns>
        public IFutureValue<SNProfile> GetOneBySMIId(int id)
        {
            var query = new DefaultQueryOver<SNProfile, int>().GetQueryOver().Where(x => x.SubModuleItem.Id == id);
            return this.Repository.FindOne(query);
        }

        /// <summary>
        /// The get one by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNProfile}"/>.
        /// </returns>
        // ReSharper disable ImplicitlyCapturedClosure
        public IEnumerable<SNProfileExtraDTO> GetAllByUserId(int userId)
        {
            SNProfile profile = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u = null;
            User u2 = null;
            SNProfileExtraFromStoredProcedureDTO dto = null;
            const UserStatus ActiveStatus = UserStatus.Active;
            return
                this.Repository.FindAll<SNProfileExtraFromStoredProcedureDTO>(
                    QueryOver.Of(() => profile)
                        .JoinAlias(x => x.SubModuleItem, () => smi)
                        .Where(() => smi.IsActive == true)
                        .JoinAlias(() => smi.SubModuleCategory, () => smc)
                        .Where(() => smc.IsActive == true)
                        .JoinAlias(() => smc.User, () => u)
                        .JoinAlias(() => smi.CreatedBy, () => u2, JoinType.LeftOuterJoin)
                        .Where(() => u2.Id == userId && u2.Status == ActiveStatus)
                        .SelectList(
                            list =>
                            list.Select(() => smc.Id)
                                .WithAlias(() => dto.subModuleCategoryId)
                                .Select(() => smc.CategoryName)
                                .WithAlias(() => dto.categoryName)
                                .Select(() => smi.Id)
                                .WithAlias(() => dto.subModuleItemId)
                                .Select(() => smi.DateModified)
                                .WithAlias(() => dto.dateModified)
                                .Select(() => smi.CreatedBy.Id)
                                .WithAlias(() => dto.createdBy)
                                .Select(() => u2.FirstName)
                                .WithAlias(() => dto.createdByFirstName)
                                .Select(() => u2.LastName)
                                .WithAlias(() => dto.createdByLastName)
                                .Select(() => u.Id)
                                .WithAlias(() => dto.userId)
                                .Select(() => u.FirstName)
                                .WithAlias(() => dto.firstName)
                                .Select(() => u.LastName)
                                .WithAlias(() => dto.lastName)
                                .Select(() => profile.Id)
                                .WithAlias(() => dto.snProfileId)
                                .Select(() => profile.ProfileName)
                                .WithAlias(() => dto.profileName))
                        .TransformUsing(Transformers.AliasToBean<SNProfileExtraFromStoredProcedureDTO>()))
                    .ToList()
                    .Select(x => new SNProfileExtraDTO(x))
                    .ToList();
        }

        /// <summary>
        /// The get one by SMI id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{SNProfile}"/>.
        /// </returns>
        public IEnumerable<SNProfileExtraDTO> GetAllSharedByUserId(int userId)
        {
            SNProfile profile = null;
            SubModuleItem smi = null;
            SubModuleCategory smc = null;
            User u = null;
            User u2 = null;
            SNProfileExtraFromStoredProcedureDTO dto = null;
            const UserStatus ActiveStatus = UserStatus.Active;
            var user = this.userModel.GetOneById(userId).Value;
            var companyId = user.Company.Id;

            return
                this.Repository.FindAll<SNProfileExtraFromStoredProcedureDTO>(
                    QueryOver.Of(() => profile)
                        .JoinAlias(x => x.SubModuleItem, () => smi)
                        .Where(() => smi.IsActive == true && smi.IsShared == true)
                        .JoinAlias(() => smi.SubModuleCategory, () => smc)
                        .Where(() => smc.IsActive == true)
                        .JoinAlias(() => smc.User, () => u)
                        .JoinAlias(() => smi.CreatedBy, () => u2)
                        .Where(() => u2.Id != userId && u2.Status == ActiveStatus && u2.Company.Id == companyId)
                        .SelectList(
                            list =>
                            list.Select(() => smc.Id)
                                .WithAlias(() => dto.subModuleCategoryId)
                                .Select(() => smc.CategoryName)
                                .WithAlias(() => dto.categoryName)
                                .Select(() => smi.Id)
                                .WithAlias(() => dto.subModuleItemId)
                                .Select(() => smi.DateModified)
                                .WithAlias(() => dto.dateModified)
                                .Select(() => u2.Id)
                                .WithAlias(() => dto.createdBy)
                                .Select(() => u2.FirstName)
                                .WithAlias(() => dto.createdByFirstName)
                                .Select(() => u2.LastName)
                                .WithAlias(() => dto.createdByLastName)
                                .Select(() => u.Id)
                                .WithAlias(() => dto.userId)
                                .Select(() => u.FirstName)
                                .WithAlias(() => dto.firstName)
                                .Select(() => u.LastName)
                                .WithAlias(() => dto.lastName)
                                .Select(() => profile.Id)
                                .WithAlias(() => dto.snProfileId)
                                .Select(() => profile.ProfileName)
                                .WithAlias(() => dto.profileName))
                        .TransformUsing(Transformers.AliasToBean<SNProfileExtraFromStoredProcedureDTO>()))
                    .ToList()
                    .Select(x => new SNProfileExtraDTO(x));
        }
        // ReSharper restore ImplicitlyCapturedClosure

        /// <summary>
        /// The get social network profiles.
        /// </summary>
        /// <param name="reportsIds">
        /// The reports ids.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary{Int32, RecentReportDTO}"/>.
        /// </returns>
        public Dictionary<int, RecentReportDTO> GetSNProfiles(List<int> reportsIds)
        {
            RecentReportDTO dto = null;
            return
                this.Repository.FindAll<RecentReportDTO>(
                    new DefaultQueryOver<SNProfile, int>().GetQueryOver()
                                                     .WhereRestrictionOn(x => x.SubModuleItem.Id)
                                                     .IsIn(reportsIds)
                                                     .SelectList(
                                                         list =>
                                                         list.Select(x => x.ProfileName)
                                                             .WithAlias(() => dto.name)
                                                             .Select(x => x.SubModuleItem.Id)
                                                             .WithAlias(() => dto.subModuleItemId))
                                                     .TransformUsing(Transformers.AliasToBean<RecentReportDTO>()))
                    .ToDictionary(x => x.subModuleItemId, x => x);
        }
    }
}