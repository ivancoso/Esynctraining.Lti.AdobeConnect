namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The SNGroupDiscussionDTO validator.
    /// </summary>
    public sealed class SNLinkDTOValidator : AbstractValidator<SNLinkDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNLinkDTOValidator"/> class.
        /// </summary>
        public SNLinkDTOValidator()
        {
            this.RuleFor(model => model.linkName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Link name is empty");
        }
    }
}