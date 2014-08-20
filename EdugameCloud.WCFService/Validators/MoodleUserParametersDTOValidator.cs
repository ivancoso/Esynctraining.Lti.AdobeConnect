namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    public class MoodleUserParametersDTOValidator : AbstractValidator<MoodleUserParametersDTO>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MoodleUserParametersDTOValidator" /> class.
        /// </summary>
        public MoodleUserParametersDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.acId)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "AC id is empty");
            this.RuleFor(model => model.domain)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Domain is empty");
            this.RuleFor(model => model.provider)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Provider is empty");
            this.RuleFor(model => model.wstoken)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Wstoken is empty");

        }

        #endregion
    }
}