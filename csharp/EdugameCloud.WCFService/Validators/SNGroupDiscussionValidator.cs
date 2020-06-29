namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The SNGroupDiscussionDTO validator.
    /// </summary>
    public sealed class SNGroupDiscussionDTOValidator : AbstractValidator<SNGroupDiscussionDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNGroupDiscussionDTOValidator"/> class.
        /// </summary>
        public SNGroupDiscussionDTOValidator()
        {
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "AC session id is empty");
            this.RuleFor(model => model.groupDiscussionTitle).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Group discussion title is empty");
            this.RuleFor(model => model.groupDiscussionData).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Group discussion data is empty");
        }
    }
}