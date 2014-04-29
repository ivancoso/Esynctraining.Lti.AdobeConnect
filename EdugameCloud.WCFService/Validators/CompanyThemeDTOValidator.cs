namespace EdugameCloud.WCFService.Validators
{
    using EdugameCloud.Core.Business.Models;
    using EdugameCloud.Core.Domain.DTO;
    using EdugameCloud.Core.Domain.Entities;

    using Esynctraining.Core.Enums;
    using Esynctraining.Core.Extensions;

    using FluentValidation;

    /// <summary>
    /// The SubModuleItem DTO validator.
    /// </summary>
    public class CompanyThemeDTOValidator : AbstractValidator<CompanyThemeDTO>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyThemeDTOValidator"/> class.
        /// </summary>
        /// <param name="companyModel">
        /// The company Model.
        /// </param>
        public CompanyThemeDTOValidator(CompanyModel companyModel)
        {
            Company company = null;
            this.RuleFor(model => model.companyId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotEmpty()
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company is empty")
                .Must(x => (company = companyModel.GetOneById(x).Value) != null)
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company doesn't exist")
                .Must(x => (company.CurrentLicense ?? company.FutureActiveLicense).With(cl => cl.LicenseStatus == CompanyLicenseStatus.Enterprise))
                .WithError(Errors.CODE_ERRORTYPE_INVALID_OBJECT, "Company is not enterprise");
        }
    }
}