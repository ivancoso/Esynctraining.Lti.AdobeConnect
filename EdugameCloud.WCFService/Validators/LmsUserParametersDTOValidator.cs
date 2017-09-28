namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Lti.Core.Business.Models;
    using EdugameCloud.Lti.DTO;
    using Esynctraining.Core.Enums;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The LMS user parameters DTO validator.
    /// </summary>
    public sealed class LmsUserParametersDTOValidator : AbstractValidator<LmsUserParametersDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersDTOValidator"/> class. 
        /// </summary>
        /// <param name="lmsUserModel">
        /// The LMS User Model.
        /// </param>
        public LmsUserParametersDTOValidator(LmsUserModel lmsUserModel)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;

            RuleFor(model => model.AcId)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "AC id is empty");

            RuleFor(model => model.Domain)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Domain is empty");

            RuleFor(model => model.provider)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Provider is empty");

            RuleFor(model => model.WsToken)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Wstoken is empty");

            RuleFor(model => model.LmsUserId)
                .Must(x => x == null || x == 0 || lmsUserModel.GetOneById(x.Value).Value != null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Specified LMS user does not exist");
        }

    }

}