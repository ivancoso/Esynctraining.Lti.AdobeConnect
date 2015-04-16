namespace EdugameCloud.WCFService
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceModel.Activation;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;
    using EdugameCloud.WCFService.Base;
    using EdugameCloud.WCFService.Contracts;

    using Esynctraining.Core.Domain.Entities;
    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Utils;

    using FluentValidation.Results;

    using Resources;

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
        ///     The <see cref="ThemeDTO" />.
        /// </returns>
        public ThemeDTO[] GetAll()
        {
            return this.ThemeModel.GetAll().ToList().Select(x => new ThemeDTO(x)).ToArray();
        }

        /// <summary>
        /// The save update.
        /// </summary>
        /// <param name="resultDto">
        /// The user.
        /// </param>
        /// <returns>
        /// The <see cref="ThemeDTO"/>.
        /// </returns>
        public ThemeDTO Save(ThemeDTO resultDto)
        {
            ValidationResult validationResult;
            if (this.IsValid(resultDto, out validationResult))
            {
                var themeModel = this.ThemeModel;
                var isTransient = resultDto.themeId == 0;
                var theme = isTransient ? null : themeModel.GetOneById(resultDto.themeId).Value;
                theme = this.ConvertDto(resultDto, theme);
                themeModel.RegisterSave(theme);
                return new ThemeDTO(theme);
            }

            var error = this.GenerateValidationError(validationResult);
            this.LogError("Theme.Save", error);
            throw new FaultException<Error>(error, error.errorMessage);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="ThemeDTO"/>.
        /// </returns>
        public ThemeDTO GetById(int id)
        {
            Theme distractor;
            if ((distractor = this.ThemeModel.GetOneById(id).Value) == null)
            {
                var error = new Error(Errors.CODE_ERRORTYPE_INVALID_OBJECT, ErrorsTexts.GetResultError_Subject, ErrorsTexts.GetResultError_NotFound);
                this.LogError("Theme.GetById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            return new ThemeDTO(distractor);
        }

        /// <summary>
        /// The delete by id.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int DeleteById(int id)
        {
            Theme entity;
            var model = this.ThemeModel;
            if ((entity = model.GetOneById(id).Value) == null)
            {
                var error = new Error(
                    Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    ErrorsTexts.GetResultError_Subject,
                    ErrorsTexts.GetResultError_NotFound);
                this.LogError("Test.DeleteById", error);
                throw new FaultException<Error>(error, error.errorMessage);
            }

            model.RegisterDelete(entity, true);
            return id;
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
        /// The <see cref="PagedThemeDTO"/>.
        /// </returns>
        public PagedThemeDTO GetPaged(int pageIndex, int pageSize)
        {
            int totalCount;
            return new PagedThemeDTO
            {
                objects = this.ThemeModel.GetAllPaged(pageIndex, pageSize, out totalCount).Select(x => new ThemeDTO(x)).ToArray(),
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
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.ModifiedBy = q.modifiedBy.HasValue ? this.UserModel.GetOneById(q.modifiedBy.Value).Value : null;
            instance.CreatedBy = q.createdBy.HasValue ? this.UserModel.GetOneById(q.createdBy.Value).Value : null;
            return instance;
        }

        #endregion
    }
}