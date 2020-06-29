namespace EdugameCloud.Core.Converters
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    ///     The company theme DTO converter.
    /// </summary>
    public sealed class CompanyThemeDTOConverter : BaseConverter<CompanyThemeDTO, CompanyTheme>
    {
        #region Fields

        /// <summary>
        /// The company theme model.
        /// </summary>
        private readonly CompanyThemeModel companyThemeModel;

        /// <summary>
        /// The file model.
        /// </summary>
        private readonly FileModel fileModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyThemeDTOConverter"/> class.
        /// </summary>
        /// <param name="companyThemeModel">
        /// The company Theme Model.
        /// </param>
        /// <param name="fileModel">
        /// The file Model.
        /// </param>
        public CompanyThemeDTOConverter(
            CompanyThemeModel companyThemeModel, 
            FileModel fileModel)
        {
            this.companyThemeModel = companyThemeModel;
            this.fileModel = fileModel;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The convert DTO and save.
        /// </summary>
        /// <param name="dto">
        /// The DTO.
        /// </param>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <param name="flushUpdates">
        /// The flush Updates.
        /// </param>
        /// <returns>
        /// The <see cref="SubModuleItem"/>.
        /// </returns>
        public override CompanyTheme Convert(CompanyThemeDTO dto, CompanyTheme instance, bool flushUpdates = false)
        {
            instance = instance ?? new CompanyTheme();
            instance.HeaderBackgroundColor = dto.headerBackgroundColor;
            instance.ButtonColor = dto.buttonColor;
            instance.ButtonTextColor = dto.buttonTextColor;
            instance.GridHeaderTextColor = dto.gridHeaderTextColor;
            instance.GridHeaderBackgroundColor = dto.gridHeaderBackgroundColor;
            instance.LoginHeaderTextColor = dto.loginHeaderTextColor;
            instance.GridRolloverColor = dto.gridRolloverColor;

            instance.PopupHeaderBackgroundColor = dto.popupHeaderBackgroundColor;
            instance.PopupHeaderTextColor = dto.popupHeaderTextColor;
            instance.QuestionColor = dto.questionColor;
            instance.QuestionHeaderColor = dto.questionHeaderColor;
            instance.WelcomeTextColor = dto.welcomeTextColor;

            instance.Logo = dto.logoId.HasValue ? this.fileModel.GetOneById(dto.logoId.Value).Value : null;

            if (flushUpdates)
            {
                this.companyThemeModel.RegisterSave(instance);
            }

            return instance;
        }

        #endregion
    }
}