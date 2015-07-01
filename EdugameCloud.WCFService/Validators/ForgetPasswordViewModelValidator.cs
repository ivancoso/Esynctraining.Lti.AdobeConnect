namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.Entities;
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
            User user;
            CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(x => x.Email)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_LOGIN, "Email is empty")
                .EmailAddress()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_LOGIN, "Email is invalid")
                .Must(x => (user = userModel.GetOneByEmail(x).Value) != null && (user.Status == UserStatus.Active || user.Status == UserStatus.Activating))
                .WithError(Errors.CODE_ERRORTYPE_USER_EXISTING, "User does not exist");
        }
    }
}