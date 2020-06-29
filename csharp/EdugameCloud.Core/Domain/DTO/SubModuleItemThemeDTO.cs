namespace EdugameCloud.Core.Domain.DTO
{
    using System;
    using System.Runtime.Serialization;

    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Extensions;

    /// <summary>
    ///     The sub module item theme DTO.
    /// </summary>
    [DataContract]
    public class SubModuleItemThemeDTO
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SubModuleItemThemeDTO" /> class.
        /// </summary>
        public SubModuleItemThemeDTO()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SubModuleItemThemeDTO"/> class. 
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        public SubModuleItemThemeDTO(SubModuleItemTheme result)
        {
            this.subModuleItemId = result.With(x => x.SubModuleItem.Id);
            this.bgImageId = result.Return(x => x.BackgroundImage.Return(i => i.Id, (Guid?)null), null);
            this.bgColor = result.BackgroundColor;
            this.correctColor = result.CorrectColor;
            this.hintColor = result.HintColor;
            this.incorrectColor = result.IncorrectColor;
            this.questionColor = result.QuestionColor;
            this.selectionColor = result.SelectionColor;
            this.titleColor = result.TitleColor;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        [DataMember]
        public string bgColor { get; set; }

        /// <summary>
        /// Gets or sets the background image id.
        /// </summary>
        [DataMember]
        public Guid? bgImageId { get; set; }

        /// <summary>
        /// Gets or sets the correct color.
        /// </summary>
        [DataMember]
        public string correctColor { get; set; }

        /// <summary>
        /// Gets or sets the hint color.
        /// </summary>
        [DataMember]
        public string hintColor { get; set; }

        /// <summary>
        /// Gets or sets the incorrect color.
        /// </summary>
        [DataMember]
        public string incorrectColor { get; set; }

        /// <summary>
        /// Gets or sets the instruction color.
        /// </summary>
        [DataMember]
        public string instructionColor { get; set; }

        /// <summary>
        /// Gets or sets the question color.
        /// </summary>
        [DataMember]
        public string questionColor { get; set; }

        /// <summary>
        /// Gets or sets the selection color.
        /// </summary>
        [DataMember]
        public string selectionColor { get; set; }

        /// <summary>
        /// Gets or sets the sub module item.
        /// </summary>
        [DataMember]
        public int subModuleItemId { get; set; }

        /// <summary>
        /// Gets or sets the title color.
        /// </summary>
        [DataMember]
        public string titleColor { get; set; }

        #endregion
    }
}