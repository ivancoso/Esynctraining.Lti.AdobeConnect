namespace EdugameCloud.WCFService.Validators
{
    using System;
    using System.Linq;

    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;
    using Esynctraining.Core.Utils;

    using FluentValidation;
    using FluentValidation.Results;

    /// <summary>
    ///     The company DTO validator.
    /// </summary>
    public class CompanyDTOValidator : AbstractValidator<CompanyDTO>
    {
        #region Fields

        /// <summary>
        /// The last primary contact error.
        /// </summary>
        private string lastPrimaryContactError;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CompanyDTOValidator" /> class.
        /// </summary>
        public CompanyDTOValidator(IValidator<UserDTO> primaryContactValidator)
        {
            this.CascadeMode = CascadeMode.StopOnFirstFailure;
            this.RuleFor(model => model.companyName)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company name is empty");
            this.RuleFor(model => model.dateCreated)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date created is empty");
            this.RuleFor(model => model.dateModified)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date modified is empty");
            this.RuleFor(model => model.licenseVO)
                .Must(x => x == null || (x.createdBy != default(int) && x.totalLicensesCount != default(int)))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT,
                    "Invalid license (licenseVo.createdBy == 0, or licenseVo.totalLicensesCount == 0)");
            this.RuleFor(model => model.primaryContactId)
                .Must((model, x) => x.HasValue || model.primaryContactVO != null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Primary contact is empty");
            this.RuleFor(model => model.primaryContactVO)
                .Must((model, x) => x == null || this.ValidatePrimaryContact(primaryContactValidator, x))
                .WithDynamicError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, () => this.lastPrimaryContactError);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get real billing error.
        /// </summary>
        /// <param name="failure">
        /// The failure.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetContactError(ValidationFailure failure)
        {
            if (!string.IsNullOrWhiteSpace(failure.ErrorMessage) && failure.ErrorMessage.Contains("#_#"))
            {
                string[] errorDetails = failure.ErrorMessage.Split(
                    new[] { "#_#" }, 
                    StringSplitOptions.RemoveEmptyEntries);
                return errorDetails.ElementAtOrDefault(1);
            }

            return failure.ErrorMessage;
        }

        /// <summary>
        /// The validate primary contact.
        /// </summary>
        /// <param name="primaryContactValidator">
        /// The primary Contact Validator.
        /// </param>
        /// <param name="userDTO">
        /// The user dto.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool ValidatePrimaryContact(IValidator<UserDTO> primaryContactValidator, UserDTO userDTO)
        {
            var res = primaryContactValidator.Validate(userDTO);
            this.lastPrimaryContactError = res.IsValid
                                               ? string.Empty
                                               : this.GetContactError(res.Errors.FirstOrDefault());
            return res.IsValid;
        }

        #endregion
    }
}