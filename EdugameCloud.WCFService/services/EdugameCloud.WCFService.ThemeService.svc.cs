// ReSharper disable CheckNamespace
namespace EdugameCloud.WCFService
// ReSharper restore CheckNamespace
{
    using System;
    using System.Linq;
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

    using FluentValidation.Results;

    using Resources;

    /// <summary>
    ///     The account service.
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class ThemeService : BaseService, IThemeService
    {
        #region Properties

        /// <summary>
        /// Gets the theme model.
        /// </summary>
        private ThemeModel ThemeModel
        {
            get
            {
                return IoC.Resolve<ThemeModel>();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     All test.
        /// </summary>
        /// <returns>
        ///     The <see cref="ServiceResponse" />.
        /// </returns>
        public ServiceResponse<ThemeDTO> GetAll()
        {
            return new ServiceResponse<ThemeDTO> { objects = this.ThemeModel.GetAll().ToList().Select(x => new ThemeDTO(x)).ToList() };
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ThemeDTO> Save(ThemeDTO resultDto)
        {
            var result = new ServiceResponse<ThemeDTO>();
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var themeModel = this.ThemeModel;
                var isTransient = resultDto.themeId == 0;
                var theme = isTransient ? null : themeModel.GetOneById(resultDto.themeId).Value;
                theme = this.ConvertDto(resultDto, theme);
                themeModel.RegisterSave(theme);
                result.@object = new ThemeDTO(theme);
                return result;
            }

            result = (ServiceResponse<ThemeDTO>)this.UpdateResult(result, validationResult);
            this.LogError(ErrorsTexts.EntityCreationError_Subject, result, string.Empty);
            return result;
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ThemeDTO> GetById(int id)
        {
            var result = new ServiceResponse<ThemeDTO>();
            Theme distractor;
            if ((distractor = this.ThemeModel.GetOneById(id).Value) == null)
            {
                result.SetError(new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = new ThemeDTO(distractor);
            }

            return result;
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<int> DeleteById(int id)
        {
            var result = new ServiceResponse<int>();
            Theme entity;
            var model = this.ThemeModel;
            if ((entity = model.GetOneById(id).Value) == null)
            {
                result.SetError(
                    new Error(
                        Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                        ErrorsTexts.GetResultError_Subject,
                        ErrorsTexts.GetResultError_NotFound));
            }
            else
            {
                model.RegisterDelete(entity, true);
                result.status = Errors.CODE_RESULTTYPE_SUCCESS;
                result.@object = id;
            }

            return result;
        }

        /// <summary>
        /// The get paged.
        /// </summary>
        /// <param name="pageIndex">
        /// The page index.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <returns>
        /// The <see cref="ServiceResponse"/>.
        /// </returns>
        public ServiceResponse<ThemeDTO> GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new ServiceResponse<ThemeDTO>
            {
                objects = this.ThemeModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new ThemeDTO(x)).ToList(),
                totalCount = totalCount
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// The convert DTO.
        /// </summary>
        /// <param name="q">
        /// The result DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <returns>
        /// The <see cref="Theme"/>.
        /// </returns>
        private Theme ConvertDto(ThemeDTO q, Theme instance)
        {
            instance = instance ?? new Theme();
            instance.IsActive = q.isActive;
            instance.ThemeName = q.themeName;
            instance.DateModified = q.dateModified == DateTime.MinValue ? DateTime.Now : q.dateModified;
            instance.DateCreated = q.dateCreated == DateTime.MinValue ? DateTime.Now : q.dateCreated;
            instance.ModifiedBy = q.modifiedBy.HasValue ? this.UserModel.GetOneById(q.modifiedBy.Value).Value : null;
            instance.CreatedBy = q.createdBy.HasValue ? this.UserModel.GetOneById(q.createdBy.Value).Value : null;
            return instance;
        }

        #endregion
    }
}