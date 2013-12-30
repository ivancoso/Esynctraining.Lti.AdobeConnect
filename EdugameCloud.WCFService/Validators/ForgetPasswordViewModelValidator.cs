namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.WCFService.ViewModels;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The forget password view model validator.
    /// </summary>
    public class ForgetPasswordViewModelValidator : AbstractValidator<ForgetPasswordViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ForgetPasswordViewModelValidator"/> class.
        /// </summary>
        /// <param name="userModel">
        /// The user model.
        /// </param>
        public ForgetPasswordViewModelValidator(UserModel userModel)
        {
            CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(x => x.Email)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_LOGIN, "Email is empty")
                .EmailAddress()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_LOGIN, "Email is invalid")
                .Must(x => userModel.GetOneByEmail(x).Value != null)
                .WithError(Errors.CODE_ERRORTYPE_USER_EXISTING, "User do not exist");
        }
    }
}