namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    public class MoodleUserDTOValidator : AbstractValidator<MoodleUserDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MoodleUserDTOValidator"/> class.
        /// </summary>
        public MoodleUserDTOValidator()
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.moodleUserId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Moodle user id is empty");
            this.RuleFor(model => model.userName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "User name is empty");
            this.RuleFor(model => model.firstName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "First name is empty");
            this.RuleFor(model => model.lastName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Last name is empty");
            this.RuleFor(model => model.password).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Password is empty");
        }
    }
}