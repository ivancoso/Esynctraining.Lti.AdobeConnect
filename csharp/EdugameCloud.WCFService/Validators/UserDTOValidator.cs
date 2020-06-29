namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.FluentValidation;
    using FluentValidation;

    /// <summary>
    /// The user DTO validator.
    /// </summary>
    public sealed class UserDTOValidator : AbstractValidator<UserDTO>
    {
        /// <summary>
        /// The user model.
        /// </summary>
        private readonly UserModel userModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserDTOValidator"/> class.
        /// </summary>
        /// <param name="userModel">
        /// The user Model.
        /// </param>
        public UserDTOValidator(UserModel userModel)
        {
            this.userModel = userModel;
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.firstName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Empty first name");
            this.RuleFor(model => model.lastName).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Empty last name");
            this.RuleFor(model => model.email)
                .NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Empty email")
                .Must((model, x) => this.ValidateEmailAlreadyExists(model.userId, x)).WithError(Errors.CODE_ERRORTYPE_USER_EXISTING, "Email already exists");
//            this.RuleFor(model => model.password).Must((model, x) => model.userId != 0 || !string.IsNullOrWhiteSpace(x)).WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Password is empty for new user");
//            this.RuleFor(model => model.companyId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Company is empty");
            this.RuleFor(model => model.languageId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "Language is empty");
            this.RuleFor(model => model.timeZoneId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "TimeZone is empty");
            this.RuleFor(model => model.userRoleId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_USER, "UserRole is empty");
        }

        /// <summary>
        /// The validate email already exists.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="email">
        /// The email.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool ValidateEmailAlreadyExists(int id, string email)
        {
            User user = this.userModel.GetOneByEmail(email).With(x => x.Value);

            if (user == null || user.Id == id)
            {
                return true;
            }

            return false;
        }

    }
}