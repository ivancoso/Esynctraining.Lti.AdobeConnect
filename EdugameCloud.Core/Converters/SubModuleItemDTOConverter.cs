namespace EdugameCloud.Core.Converters
{
    using System;
    using System.Linq;

    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    /// <summary>
    /// The sub module item DTO converter.
    /// </summary>
    public sealed class SubModuleItemDTOConverter : BaseConverter<SubModuleItemDTO, SubModuleItem>
    {
        #region Fields

        /// <summary>
        /// The sub module item model.
        /// </summary>
        private readonly SubModuleItemModel subModuleItemModel;

        /// <summary>
        /// The user model.
        /// </summary>
        private readonly UserModel userModel;

        /// <summary>
        /// The sub module category model.
        /// </summary>
        private readonly SubModuleCategoryModel subModuleCategoryModel;

        /// <summary>
        /// The sub module item theme model.
        /// </summary>
        private readonly SubModuleItemThemeModel subModuleItemThemeModel;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemDTOConverter"/> class.
        /// </summary>
        /// <param name="subModuleCategoryModel">
        /// The sub Module Category Model.
        /// </param>
        /// <param name="subModuleItemThemeModel">
        /// The sub module item theme model.
        /// </param>
        /// <param name="subModuleItemModel">
        /// The sub module item model.
        /// </param>
        /// <param name="userModel">
        /// The user Model.
        /// </param>
        public SubModuleItemDTOConverter(
            SubModuleCategoryModel subModuleCategoryModel,
            SubModuleItemThemeModel subModuleItemThemeModel, 
            SubModuleItemModel subModuleItemModel,
            UserModel userModel)
        {
            this.subModuleCategoryModel = subModuleCategoryModel;
            this.subModuleItemThemeModel = subModuleItemThemeModel;
            this.subModuleItemModel = subModuleItemModel;
            this.userModel = userModel;
        }

        #endregion

        #region Methods

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
        public override SubModuleItem Convert(SubModuleItemDTO dto, SubModuleItem instance, bool flushUpdates = false)
        {
            instance = instance ?? new SubModuleItem();
            instance.IsActive = dto.isActive;
            instance.IsShared = dto.isShared;
            if (instance.IsTransient())
            {
                instance.DateCreated = DateTime.Now;
            }

            instance.DateModified = DateTime.Now;
            instance.SubModuleCategory = this.subModuleCategoryModel.GetOneById(dto.subModuleCategoryId).Value;
            instance.CreatedBy = dto.createdBy.HasValue ? this.userModel.GetOneById(dto.createdBy.Value).Value : null;
            instance.ModifiedBy = dto.modifiedBy.HasValue ? this.userModel.GetOneById(dto.modifiedBy.Value).Value : null;

            if (instance.IsTransient() || flushUpdates)
            {
                this.subModuleItemModel.RegisterSave(instance);
            }

            if (dto.themeVO != null)
            {
                SubModuleItemTheme theme = instance.Themes.FirstOrDefault() ?? new SubModuleItemTheme();
                SubModuleItemThemeDTO themeVo = dto.themeVO;
                theme.BackgroundColor = themeVo.bgColor;
                theme.HintColor = themeVo.hintColor;
                theme.CorrectColor = themeVo.correctColor;
                theme.IncorrectColor = themeVo.incorrectColor;
                theme.InstructionColor = themeVo.instructionColor;
                theme.QuestionColor = themeVo.questionColor;
                theme.SelectionColor = themeVo.selectionColor;
                theme.TitleColor = themeVo.titleColor;
                theme.SubModuleItem = instance;
                if (theme.IsTransient() || flushUpdates)
                {
                    this.subModuleItemThemeModel.RegisterSave(theme);
                }

                instance.Themes.Add(theme);
            }

            return instance;
        }

        #endregion
    }
}