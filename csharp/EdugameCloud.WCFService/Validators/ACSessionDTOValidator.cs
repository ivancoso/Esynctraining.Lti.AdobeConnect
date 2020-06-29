namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The ACSession DTO validator.
    /// </summary>
    public sealed class ACSessionValidator : AbstractValidator<ACSessionDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ACSessionValidator"/> class.
        /// </summary>
        public ACSessionValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.accountId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Account id is empty");
            this.RuleFor(model => model.status).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Status is empty");
            this.RuleFor(model => model.meetingURL).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Meeting url is empty");
            this.RuleFor(model => model.scoId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "ScoId is empty");
            this.RuleFor(model => model.languageId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Language is empty");
            this.RuleFor(model => model.userId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "User is empty");
            this.RuleFor(model => model.subModuleItemId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Sub module item is empty");
            this.RuleFor(model => model.acUserModeId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Ac user mode is empty");
        }

    }
}