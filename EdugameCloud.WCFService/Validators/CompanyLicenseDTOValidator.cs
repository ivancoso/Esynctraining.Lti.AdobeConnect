namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Domain.DTO;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The company license DTO validator.
    /// </summary>
    public class CompanyLicenseDTOValidator : AbstractValidator<CompanyLicenseDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyLicenseDTOValidator"/> class.
        /// </summary>
        public CompanyLicenseDTOValidator()
        {
            this.RuleFor(model => model.companyId).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company is empty");
            this.RuleFor(model => model.licenseNumber).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "License number is empty");
            this.RuleFor(model => model.expiryDate).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Expiry date is empty");
            this.RuleFor(model => model.createdBy).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Created by is empty");
            this.RuleFor(model => model.modifiedBy).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Modified by is empty");
            this.RuleFor(model => model.dateCreated).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date created is empty");
            this.RuleFor(model => model.dateModified).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Date modified is empty");
            this.RuleFor(model => model.totalLicensesCount).NotEmpty().WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Total licenses count can't be 0");
        }

    }
}