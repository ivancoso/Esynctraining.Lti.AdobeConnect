namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;

    using FluentValidation;

    /// <summary>
    /// The SNSessionMemberDTO validator.
    /// </summary>
    public sealed class SNMemberDTOValidator : AbstractValidator<SNMemberDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SNMemberDTOValidator"/> class.
        /// </summary>
        public SNMemberDTOValidator()
        {
            this.RuleFor(model => model.participant).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Participant name is empty");
//            this.RuleFor(model => model.participantProfile).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Participant profile is empty");
            this.RuleFor(model => model.acSessionId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "AC session id is empty");
        }
    }
}