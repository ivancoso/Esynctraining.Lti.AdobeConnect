namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Lti.Business.Models;
    using EdugameCloud.Lti.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The LMS user parameters DTO validator.
    /// </summary>
    public class LmsUserParametersDTOValidator : AbstractValidator<LmsUserParametersDTO>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LmsUserParametersDTOValidator"/> class. 
        /// </summary>
        /// <param name="lmsUserModel">
        /// The LMS User Model.
        /// </param>
        public LmsUserParametersDTOValidator(LmsUserModel lmsUserModel)
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

            this.RuleFor(model => model.lmsUserId)
                .Must(x => x == null || x == 0 || lmsUserModel.GetOneById(x.Value).Value != null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Specified LMS user does not exist");
        }

        #endregion
    }
}